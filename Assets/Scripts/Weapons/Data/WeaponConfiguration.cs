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
        StopShooting(shootSystem);
        AmmoConfig.Reload();
        AudioConfig.PlayReloadingClip(shootSystem.GetComponent<AudioSource>(), ShootConfig.IsAutomaticFire);
    }

    public void UpdateWeaponBehaviour(ParticleSystem shootSystem)
    {
        if(AmmoConfig.ClipAmmo > 0)
        { 
            Shoot(shootSystem);
        }
        else if(AmmoConfig.ClipAmmo == 0)
        {
            StopShooting(shootSystem);
            AudioConfig.PlayOutOfAmmoClip(shootSystem.GetComponent<AudioSource>(), ShootConfig.IsAutomaticFire);
            return;
        }
    }

    private void Shoot(ParticleSystem shootSystem)
    {
        if (Time.time > ShootConfig.FireRate + _lastShootTime)
        {
            _lastShootTime = Time.time;
            AudioSource audioSource = shootSystem.GetComponent<AudioSource>();

            shootSystem.Play();
            AudioConfig.PlayShootingClip(audioSource, AmmoConfig.ClipAmmo == 1, ShootConfig.IsAutomaticFire, ShootConfig.FireRate);
            HandleRecoil(shootSystem.transform.parent.parent, AmmoConfig.ClipAmmo < 0);

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
        }
    }

    private void StopShooting(ParticleSystem shootSystem)
    {
        if (shootSystem.isPlaying)
        {
            shootSystem.Stop();
        }

        if (_isRecoiling && _recoilRoutine != null)
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

    private void HandleRecoil(Transform weaponTransform, bool isClipEmpty)
    {
        if(isClipEmpty)
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

        Vector3 originalPosition = weaponTransform.localPosition;
        Vector3 recoilPosition = originalPosition + new Vector3(ShootConfig.Recoil, 0, 0); 

        float elapsedTime = 0;

        while (elapsedTime < ShootConfig.RecoilSpeed)
        {
            weaponTransform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, elapsedTime / ShootConfig.RecoilSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponTransform.localPosition = recoilPosition;

        elapsedTime = 0;
        while (elapsedTime < ShootConfig.RecoilReturnSpeed)
        {
            weaponTransform.localPosition = Vector3.Lerp(recoilPosition, originalPosition, elapsedTime / ShootConfig.RecoilReturnSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponTransform.localPosition = originalPosition;
        _isRecoiling = false;
    }

}
