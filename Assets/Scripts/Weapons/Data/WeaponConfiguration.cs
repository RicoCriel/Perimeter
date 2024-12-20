using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.UI;

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
    private bool _isReloading;
    private bool _shouldAutoReload = true;

    private bool _hasPlayedEmptyClip;

    private Vector3 _originalPosition;
    private ObjectPool<TrailRenderer> _trailPool;
    private Coroutine _recoilRoutine;

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

    public void FireWeapon(ParticleSystem shootSystem, bool wantsToShoot, Image aimCrosshair, Transform rigAimingTarget)
    {
        AudioSource audioSource = shootSystem.GetComponent<AudioSource>();

        if (!wantsToShoot)
        {
            HandleInactiveWeapon(shootSystem, audioSource);
            return;
        }

        if (AmmoConfig.ClipAmmo > 0)
        {
            Shoot(shootSystem, aimCrosshair, rigAimingTarget);
        }
        else if (AmmoConfig.ClipAmmo == 0)
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

    private void Shoot(ParticleSystem shootSystem, Image aimCrosshair, Transform rigAimingTarget)
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

            Vector3 startPosition = shootSystem.transform.position;

            // Calculate direction towards the crosshair
            Vector3 aimWorldPosition = Vector3.zero;
            if (aimCrosshair != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(aimCrosshair.rectTransform.position);
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, ShootConfig.HitMask))
                {
                    aimWorldPosition = hit.point;
                }
                else
                {
                    aimWorldPosition = ray.GetPoint(TrailConfig.MissDistance); // A fallback if no hit
                }
            }

            Vector3 shootDirection = (aimWorldPosition - startPosition).normalized + spreadDirection;

            AmmoConfig.ClipAmmo--;

            if (Physics.Raycast(startPosition, shootDirection, out RaycastHit rayHit, float.MaxValue, ShootConfig.HitMask))
            {
                _activeMonoBehaviour.StartCoroutine(PlayBulletTrail(startPosition, rayHit.point, rayHit));
            }
            else
            {
                _activeMonoBehaviour.StartCoroutine(PlayBulletTrail(
                    startPosition,
                    startPosition + (shootDirection * TrailConfig.MissDistance),
                    new RaycastHit()
                ));
            }

            // HandleRecoil(rigAimingTransform);
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
        if (AmmoConfig.ClipAmmo <= 0)
        {
            return;
        }

        // Stop any ongoing recoil routine before starting a new one
        if (_recoilRoutine != null)
        {
            _activeMonoBehaviour.StopCoroutine(_recoilRoutine);
        }

        // Start the recoil routine
        _recoilRoutine = _activeMonoBehaviour.StartCoroutine(StartRecoil(weaponTransform));
    }

    private void StopWeaponEffects(ParticleSystem shootSystem)
    {
        if (shootSystem.isPlaying)
        {
            shootSystem.Stop();
        }

        if (_recoilRoutine != null)
        {
            _activeMonoBehaviour.StopCoroutine(_recoilRoutine);
            _recoilRoutine = null;
        }
    }

    private IEnumerator StartRecoil(Transform weaponTransform)
    {
        //This logic is FUCKED
        _originalPosition = weaponTransform.localPosition;
        Vector3 offset = new Vector3(ShootConfig.Recoil, 0, 0);
        Vector3 recoilPosition = _originalPosition + offset;

        float elapsedTime = 0;

        // Move weapon to recoil position
        while (elapsedTime < ShootConfig.RecoilSpeed)
        {
            weaponTransform.localPosition = Vector3.Lerp(_originalPosition, recoilPosition, elapsedTime / ShootConfig.RecoilSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponTransform.localPosition = recoilPosition;
        elapsedTime = 0;

        // Return weapon to original position
        while (elapsedTime < ShootConfig.RecoilReturnSpeed)
        {
            weaponTransform.localPosition = Vector3.Lerp(recoilPosition, _originalPosition, elapsedTime / ShootConfig.RecoilReturnSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponTransform.localPosition = _originalPosition;

        // Clear the coroutine reference once recoil completes
        _recoilRoutine = null;
    }

    public void ReloadWeapon(WeaponConfiguration activeWeaponConfig, InputAction.CallbackContext context)
    {
        if(AutoReload(activeWeaponConfig) || ManualReload(activeWeaponConfig, context))
        { 
            Reload(activeWeaponConfig);
        }
    }

    private bool AutoReload(WeaponConfiguration activeWeaponConfig)
    {
        return !_isReloading
            && _shouldAutoReload
            && activeWeaponConfig.AmmoConfig.ClipAmmo == 0
            && activeWeaponConfig.CanReload();
    }

    private bool ManualReload(WeaponConfiguration activeWeaponConfig, InputAction.CallbackContext context)
    {
        return !_isReloading
            && PlayerHelper.IsInputPressed(context.ReadValue<float>())
            && activeWeaponConfig.CanReload();
    }

    private void Reload(WeaponConfiguration activeWeaponConfig)
    {
        activeWeaponConfig.Reload(WeaponInventory.Instance.ActiveWeaponShootSystem);
        _isReloading = false;
    }

    private void StartReload()
    {
        _isReloading = true;
    }

    private IEnumerator ReloadRoutine(WeaponConfiguration activeWeaponConfig)
    {
        float elapsedTime = 0f;
        float reloadDuration = activeWeaponConfig.AmmoConfig.ReloadDuration;

        while (elapsedTime < reloadDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartReload();
        //_reloadRoutine = null;
    }

}
