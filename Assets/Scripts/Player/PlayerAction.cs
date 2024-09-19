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
    private Dictionary<FireMode, System.Func<bool>> _fireModeActions;

    private void Start()
    {
        _fireModeActions = new Dictionary<FireMode, System.Func<bool>>
        {
            { FireMode.SingleFire, () => Input.GetMouseButtonDown(0) },
            { FireMode.AutomaticFire, () => Input.GetMouseButton(0) },
            { FireMode.StopFiring, () => Input.GetMouseButtonUp(0) },
        };
    }

    private void Update()
    {
        HandlePlayerFireInput();
    }

    private void HandlePlayerFireInput()
    {
        var activeWeaponConfig = WeaponInventory.Instance.ActiveWeaponConfig;
        var shootConfig = activeWeaponConfig.ShootConfig;

        if (shootConfig.IsSingleFire && _fireModeActions[FireMode.SingleFire]())
        {
            activeWeaponConfig.Shoot(WeaponInventory.Instance.ActiveWeaponParticleSystem);
        }
        else if (shootConfig.IsAutomaticFire && _fireModeActions[FireMode.AutomaticFire]())
        {
            activeWeaponConfig.Shoot(WeaponInventory.Instance.ActiveWeaponParticleSystem);
        }

        if (_fireModeActions[FireMode.StopFiring]())
        {
            activeWeaponConfig.StopShooting(WeaponInventory.Instance.ActiveWeaponParticleSystem);
        }
    }
}
