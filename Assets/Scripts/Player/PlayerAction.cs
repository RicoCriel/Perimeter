using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum FireMode
{
    SingleFire,
    AutomaticFire,
    StopFiring
}

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private bool _shouldAutoReload;

    public UnityEvent OnFire;
    public UnityEvent OnStartReload;
    public UnityEvent OnFinishReload;

    private Dictionary<FireMode, Func<bool>> _fireModeInputActions;
    private bool _hasReloaded;
    private WeaponConfiguration _activeWeaponConfig;
    private Coroutine _reloadRoutine;

    private void Start()
    {
        _fireModeInputActions = new Dictionary<FireMode, Func<bool>>
        {
            { FireMode.SingleFire, () => Input.GetMouseButtonDown(0) },
            { FireMode.AutomaticFire, () => Input.GetMouseButton(0) },
            { FireMode.StopFiring, () => Input.GetMouseButtonUp(0) }
        };
    }

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
            && _shouldAutoReload
            && activeWeaponConfig.AmmoConfig.ClipAmmo == 0
            && activeWeaponConfig.CanReload();
    }

    private void FinishReload(WeaponConfiguration activeWeaponConfig)
    {
        activeWeaponConfig.Reload(WeaponInventory.Instance.ActiveWeaponParticleSystem);
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
