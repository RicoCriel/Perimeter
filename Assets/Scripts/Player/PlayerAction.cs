using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FireMode
{
    SingleFire,
    AutomaticFire,
    StopFiring
}

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private bool _shouldAutoReload;
    [SerializeField] private Animator _playerAnimator;
    private Dictionary<FireMode, System.Func<bool>> _fireModeInputActions;
    private bool _hasReloaded;
    private WeaponConfiguration _activeWeaponConfig;

    private void Start()
    {
        _fireModeInputActions = new Dictionary<FireMode, System.Func<bool>>
        {
            { FireMode.SingleFire, () => Input.GetMouseButtonDown(0) },
            { FireMode.AutomaticFire, () => Input.GetMouseButton(0) },
            { FireMode.StopFiring, () => Input.GetMouseButtonUp(0) },
        };
    }

    private void Update()
    {
        _activeWeaponConfig = WeaponInventory.Instance.ActiveWeaponConfig;
        HandlePlayerFireInput(_activeWeaponConfig);
        if (!_hasReloaded && (AutoReload(_activeWeaponConfig) || ManualReload(_activeWeaponConfig)))
        {
            _playerAnimator.SetTrigger("Reload");
        }

        if (_hasReloaded)
        {
            FinishReload(_activeWeaponConfig);
        }
    }
    private void HandlePlayerFireInput(WeaponConfiguration activeWeaponConfig)
    {
        activeWeaponConfig.Tick(GetPlayerInput(activeWeaponConfig), WeaponInventory.Instance.ActiveWeaponParticleSystem);
    }

    private bool GetPlayerInput(WeaponConfiguration activeWeaponConfig)
    {
        if (_fireModeInputActions[FireMode.SingleFire]() && activeWeaponConfig.ShootConfig.IsSingleFire)
        {
            return true;  
        }

        if (_fireModeInputActions[FireMode.AutomaticFire]() && activeWeaponConfig.ShootConfig.IsAutomaticFire)
        {
            return true;  
        }

        if (_fireModeInputActions[FireMode.StopFiring]())
        {
            return false; 
        }

        return false; 
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
            &&_shouldAutoReload
            && activeWeaponConfig.AmmoConfig.ClipAmmo == 0
            && activeWeaponConfig.CanReload();
    }

    private void FinishReload(WeaponConfiguration activeWeaponConfig)
    {
        activeWeaponConfig.Reload();
        _hasReloaded = false;
    }

    public void EndReload()
    {
        _hasReloaded = true;
    }

}



