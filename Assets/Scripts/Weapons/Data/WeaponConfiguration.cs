using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "WeaponConfiguration", menuName = "Weapons/WeaponConfiguration", order = 0)]
public class WeaponConfiguration : ScriptableObject
{
    public ImpactType ImpactType;
    public ShootConfiguration ShootConfig;
    public TrailConfiguration TrailConfig;
    public AmmoConfiguration AmmoConfig;
    public AudioConfiguration AudioConfig;

    private MonoBehaviour _activeMonoBehaviour;
    private float _lastShootTime;
    private bool _isRecoiling;
    private bool _hasPlayedEmptyClip;

    private Vector3 _originalPosition;
    private ObjectPool<TrailRenderer> _trailPool;
    private Coroutine _recoilRoutine;

    private void OnEnable()
    {
        _isRecoiling = false;
    }

    public void ActivateBulletTrail(MonoBehaviour activeMonoBehaviour)
    {
        this._activeMonoBehaviour = activeMonoBehaviour;
        _lastShootTime = 0;
        _trailPool = new ObjectPool<TrailRenderer>(CreateBulletTrail, maxSize: TrailConfig.MaxAmount);
    }

    public bool CanReload()
    {
        return AmmoConfig.CanReload();
    }

    public void Reload(ParticleSystem shootSystem)
    {
        AudioSource audioSource = shootSystem.GetComponent<AudioSource>();
        StopWeaponEffects(shootSystem);
        AmmoConfig.Reload();
        AudioConfig.PlayReloadingClip(audioSource);
        _hasPlayedEmptyClip = false;
    }

    public void FireWeapon(ParticleSystem shootSystem, bool wantsToShoot)
    {
        AudioSource audioSource = shootSystem.GetComponent<AudioSource>();

        if(!wantsToShoot)
        {
            HandleInactiveWeapon(shootSystem, audioSource);
            return;
        }

        if (AmmoConfig.ClipAmmo > 0 && wantsToShoot)
        {
            Shoot(shootSystem);
        }
        else if(AmmoConfig.ClipAmmo == 0 && wantsToShoot)
        {
            HandleEmptyWeapon(shootSystem, audioSource);
        }
    }

    private void HandleEmptyWeapon(ParticleSystem shootSystem, AudioSource audioSource)
    {
        StopWeaponEffects(shootSystem);
        AudioConfig.StopAudio(audioSource);

        if (!audioSource.isPlaying && !_hasPlayedEmptyClip)
        {
            AudioConfig.PlayOutOfAmmoClip(audioSource);
            _hasPlayedEmptyClip = true;
        }
    }

    private void HandleInactiveWeapon(ParticleSystem shootSystem, AudioSource audioSource)
    {
        _hasPlayedEmptyClip = false;
        StopWeaponEffects(shootSystem);
        if (ShootConfig.IsAutomaticFire)
        {
            AudioConfig.StopAudio(audioSource);
        }
    }

    private void Shoot(ParticleSystem shootSystem)
    {
        if (Time.time > ShootConfig.FireRate + _lastShootTime)
        {
            _lastShootTime = Time.time;
            AudioSource audioSource = shootSystem.GetComponent<AudioSource>();

            shootSystem.Play();
            AudioConfig.PlayShootingClip(audioSource, AmmoConfig.ClipAmmo == 1, ShootConfig.IsAutomaticFire);

            Vector3 spreadDirection = new Vector3(
                Random.Range(-ShootConfig.Spread.x, ShootConfig.Spread.x),
                Random.Range(-ShootConfig.Spread.y, ShootConfig.Spread.y),
                Random.Range(-ShootConfig.Spread.z, ShootConfig.Spread.z)
            );

            Vector3 shootDirection = shootSystem.transform.parent.forward + spreadDirection;
            shootDirection.Normalize();

            Vector3 startPosition = shootSystem.transform.position;
            AmmoConfig.ClipAmmo --;

            if (Physics.Raycast(startPosition, shootDirection, out RaycastHit hit, float.MaxValue, ShootConfig.HitMask))
            {
                _activeMonoBehaviour.StartCoroutine(PlayBulletTrail(startPosition, hit.point, hit));
            }
            else
            {
                _activeMonoBehaviour.StartCoroutine(PlayBulletTrail(
                    startPosition,
                    startPosition + (shootDirection * TrailConfig.MissDistance),
                    new RaycastHit() 
                    ));
            }

            HandleRecoil(shootSystem.transform.parent.parent);
        }
    }

    private void StopWeaponEffects(ParticleSystem shootSystem)
    {
        if (shootSystem.isPlaying)
        {
            shootSystem.Stop();
        }

        if (_isRecoiling || _recoilRoutine != null)
        {
            _activeMonoBehaviour.StopCoroutine(_recoilRoutine);
            _isRecoiling = false;
        }
    }

    private IEnumerator PlayBulletTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer instance = _trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        bool released = false;

        try
        {
            while (remainingDistance > 0)
            {
                instance.transform.position = Vector3.Lerp(startPoint, endPoint,
                    Mathf.Clamp01(1 - remainingDistance / distance));

                remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

                yield return null;
            }

            instance.transform.position = endPoint;

            if (hit.collider != null)
            {
                SurfaceManager.Instance.HandleImpact(hit.transform.gameObject, endPoint, hit.normal, ImpactType, 0);
            }

            yield return new WaitForSeconds(TrailConfig.Duration);
        }
        finally
        {
            if (!released)
            {
                instance.emitting = false;
                instance.gameObject.SetActive(false);
                _trailPool.Release(instance);
                released = true;
            }
        }
    }

    private TrailRenderer CreateBulletTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;
        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return trail;
    }

    private void HandleRecoil(Transform weaponTransform)
    {
        if(AmmoConfig.ClipAmmo <= 0)
        {
            return;
        }

        if (!_isRecoiling)
        {
            if (_recoilRoutine != null)
            {
                _activeMonoBehaviour.StopCoroutine(_recoilRoutine);
            }

            _recoilRoutine = _activeMonoBehaviour.StartCoroutine(StartRecoil(weaponTransform));
        }
    }

    private IEnumerator StartRecoil(Transform weaponTransform)
    {
        _isRecoiling = true;

        _originalPosition = weaponTransform.localPosition;
        Vector3 offset = new Vector3(ShootConfig.Recoil, 0, 0);
        Vector3 recoilPosition = _originalPosition + offset;

        float elapsedTime = 0;

        while (elapsedTime < ShootConfig.RecoilSpeed)
        {
            weaponTransform.localPosition = Vector3.Lerp(_originalPosition, recoilPosition, elapsedTime / ShootConfig.RecoilSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponTransform.localPosition = recoilPosition;

        while (elapsedTime < ShootConfig.RecoilReturnSpeed)
        {
            weaponTransform.localPosition = Vector3.Lerp(recoilPosition, _originalPosition, elapsedTime / ShootConfig.RecoilReturnSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponTransform.localPosition = _originalPosition;

        _isRecoiling = false;
        _recoilRoutine = null;
    }

}
