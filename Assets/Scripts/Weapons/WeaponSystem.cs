using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WeaponSystem : MonoBehaviour
{
    private MonoBehaviour _activeMonoBehaviour;
    private float _lastShootTime;
    private bool _isReloading;
    private bool _shouldAutoReload = true;

    public WeaponConfiguration WeaponConfiguration;
    public UnityEvent OnFire;
    private AmmoConfiguration _ammoConfig;
    private ShootConfiguration _shootConfig;
    private AudioConfiguration _audioConfig;
    private TrailConfiguration _trailConfig;
    private ParticleSystem _weaponParticleSystem;
    private AudioSource _audioSource;
    private Coroutine _autoFireRoutine;

    [Header("Aiming Visuals Settings")]
    [SerializeField] private Image _aimCrossHair;
    [SerializeField] private Transform _aimTarget;
    [Range(1, 100f)]
    [SerializeField] private float _crossHairSpeed;

    private void Start()
    {
        WeaponConfiguration = WeaponInventory.Instance.GetActiveWeaponConfig();
        WeaponInventory.Instance.OnWeaponChanged.AddListener(UpdateWeaponConfig);
        UpdateWeaponConfig(WeaponConfiguration);
    }

    private void UpdateWeaponConfig(WeaponConfiguration newConfig)
    {
        if (newConfig != null)
        {
            WeaponConfiguration = newConfig;

            _ammoConfig = WeaponConfiguration.AmmoConfig;
            _shootConfig = WeaponConfiguration.ShootConfig;
            _audioConfig = WeaponConfiguration.AudioConfig;
            _trailConfig = WeaponConfiguration.TrailConfig;
            _weaponParticleSystem = WeaponInventory.Instance.ActiveWeaponParticleSystem;
            _audioSource = _weaponParticleSystem.GetComponent<AudioSource>();
        }
    }

    public void FireWeapon(bool wantsToShoot, bool isAiming)
    {
        if (!wantsToShoot)
        {
            HandleInactiveWeapon();
            return;
        }

        if (_ammoConfig.ClipAmmo > 0)
        {
            Shoot(_aimCrossHair, isAiming);
        }
        else if (_ammoConfig.ClipAmmo == 0)
        {
            HandleEmptyWeapon();
        }
    }

    private void HandleEmptyWeapon()
    {
        StopWeaponEffects();
        _audioConfig.StopAudio(_audioSource);
    }

    private void HandleInactiveWeapon()
    {
        StopWeaponEffects();
    }

    private void Shoot(Image aimCrosshair, bool isAiming)
    {
        if (Time.time > _shootConfig.FireRate + _lastShootTime)
        {
            _lastShootTime = Time.time;

            _weaponParticleSystem.Play();
            _audioConfig.PlayShootingClip(_audioSource, _ammoConfig.ClipAmmo == 1, _shootConfig.IsAutomaticFire);

            Vector3 spreadDirection = new Vector3(
                Random.Range(-_shootConfig.Spread.x, _shootConfig.Spread.x),
                Random.Range(-_shootConfig.Spread.y, _shootConfig.Spread.y),
                Random.Range(-_shootConfig.Spread.z, _shootConfig.Spread.z)
            );

            Vector3 startPosition = _weaponParticleSystem.transform.position;
            Vector3 shootDirection = Vector3.zero;

            if (isAiming)
            {
                Vector3 aimWorldPosition = Vector3.zero;
                if (aimCrosshair != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(aimCrosshair.rectTransform.position);
                    if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, _shootConfig.HitMask))
                    {
                        aimWorldPosition = hit.point;
                    }
                    else
                    {
                        aimWorldPosition = ray.GetPoint(_trailConfig.MissDistance);
                    }
                }

                shootDirection = (aimWorldPosition - startPosition).normalized + spreadDirection;
            }
            else
            {
                shootDirection = _weaponParticleSystem.transform.right + spreadDirection;
            }

            _ammoConfig.ClipAmmo--;

            if (Physics.Raycast(startPosition, shootDirection, out RaycastHit rayHit, float.MaxValue, _shootConfig.HitMask))
            {
                StartCoroutine(WeaponConfiguration.PlayBulletTrail(startPosition, rayHit.point, rayHit));
            }
            else
            {
                StartCoroutine(WeaponConfiguration.PlayBulletTrail(
                    startPosition,
                    startPosition + (shootDirection * _trailConfig.MissDistance),
                    new RaycastHit()
                ));
            }
        }
    }

    public void SingleFire(float fireInput, float aimInput)
    {
        if (_shootConfig.Firemode == FIREMODE.SINGLE)
        {
            FireWeapon(PlayerHelper.IsInputPressed(fireInput), PlayerHelper.IsInputPressed(aimInput));

            if (_ammoConfig.ClipAmmo > 0 && PlayerHelper.IsInputPressed(fireInput))
            {
                OnFire?.Invoke();
            }
        }
    }

    public void AutoFire(float aimInput, bool shouldFire)
    {
        if (_shootConfig.Firemode == FIREMODE.AUTO && shouldFire)
        {
            if (_autoFireRoutine != null)
            {
                StopCoroutine(_autoFireRoutine);
            }

            if (_autoFireRoutine == null)
            {
                _autoFireRoutine = StartCoroutine(ExecuteAutoFire(aimInput));
            }
        }
    }

    public void BurstFire(float fireInput, float aimInput)
    {
        //To be implemented
    }

    private IEnumerator ExecuteAutoFire(float aimInput)
    {
        // Continue to fire the weapon while the automatic fire button is held down
        while (true)
        {
            if (_ammoConfig.ClipAmmo > 0)
            {
                OnFire?.Invoke();
            }
            FireWeapon(true, PlayerHelper.IsInputPressed(aimInput));
            yield return null;
            yield return new WaitForSeconds(_shootConfig.FireRate);
        }
    }

    private void StopWeaponEffects()
    {
        if (_weaponParticleSystem.isPlaying)
        {
            _weaponParticleSystem.Stop();
        }
    }

    public void ReloadMagazine()
    {
        StopWeaponEffects();
        _ammoConfig.Reload();
        _audioConfig.PlayReloadingClip(_audioSource);
    }

    private bool AutoReloadWeapon()
    {
        return !_isReloading
            && _shouldAutoReload
            && _ammoConfig.ClipAmmo == 0
            && WeaponConfiguration.CanReload();
    }

    private bool ManualReloadWeapon()
    {
        return !_isReloading
            && WeaponConfiguration.CanReload();
    }

    private void StartReloadMagazine()
    {
        _isReloading = true;
    }

    private void StartRecoil(Transform WeaponTransform)
    {

    }

    private void EndRecoil(Transform WeaponTransform)
    {

    }

    public void UpdateCrosshairPosition()
    {
        Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(_aimTarget.position);
        _aimCrossHair.rectTransform.position = Vector3.Lerp(
            _aimCrossHair.rectTransform.position,
            targetScreenPosition,
            Time.deltaTime * _crossHairSpeed
        );
    }
}
