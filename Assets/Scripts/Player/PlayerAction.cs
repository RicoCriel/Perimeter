using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAction : MonoBehaviour
{
    [SerializeField] private bool _shouldAutoReload;
    [SerializeField] private PlayerInput _playerInput;

    private bool _hasReloaded;
    private WeaponConfiguration _activeWeaponConfig;
    private Coroutine _reloadRoutine;

    private void Update()
    {
        _activeWeaponConfig = WeaponInventory.Instance.ActiveWeaponConfig;
        HandlePlayerFireInput(_activeWeaponConfig);

        if (_reloadRoutine == null && (!_hasReloaded && (AutoReload(_activeWeaponConfig) || ManualReload(_activeWeaponConfig))))
        {
            _reloadRoutine = StartCoroutine(ReloadRoutine(_activeWeaponConfig));
        }

        if (_hasReloaded)
        {
            FinishReload(_activeWeaponConfig);
        }
    }

    private void HandlePlayerFireInput(WeaponConfiguration activeWeaponConfig)
    {
        if (_reloadRoutine != null)
        {
            return; 
        }

        ParticleSystem shootSystem = WeaponInventory.Instance.ActiveWeaponShootSystem;

        //if(Mouse.current.leftButton.wasPressedThisFrame && activeWeaponConfig.ShootConfig.IsSingleFire)
        //{
        //    activeWeaponConfig.Tick(shootSystem);
        //}
        //else if (Mouse.current.leftButton.IsActuated(0) && activeWeaponConfig.ShootConfig.IsAutomaticFire)
        //{
        //    activeWeaponConfig.Tick(shootSystem);
        //}

        //if (Mouse.current.leftButton.wasReleasedThisFrame)
        //{
        //    activeWeaponConfig.Tick(shootSystem);
        //}
    }
    private bool ManualReload(WeaponConfiguration activeWeaponConfig)
    {
        return !_hasReloaded
            && Input.GetKeyUp(KeyCode.R)
            && activeWeaponConfig.CanReload();
    }

    private bool AutoReload(WeaponConfiguration activeWeaponConfig)
    {
        return !_hasReloaded
            && _shouldAutoReload
            && activeWeaponConfig.AmmoConfig.ClipAmmo == 0
            && activeWeaponConfig.CanReload();
    }

    private void FinishReload(WeaponConfiguration activeWeaponConfig)
    {
        activeWeaponConfig.Reload(WeaponInventory.Instance.ActiveWeaponShootSystem);
        _hasReloaded = false;
    }

    private void StartReload()
    {
        _hasReloaded = true;
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
        _reloadRoutine = null;
    }

}
