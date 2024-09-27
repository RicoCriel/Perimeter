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
    private Dictionary<FireMode, System.Func<bool>> _fireModeInputActions;

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
        HandlePlayerFireInput();
    }

    private void HandlePlayerFireInput()
    {
        var activeWeaponConfig = WeaponInventory.Instance.ActiveWeaponConfig;
        activeWeaponConfig.Tick(GetPlayerInput(), WeaponInventory.Instance.ActiveWeaponParticleSystem);
    }

    private bool GetPlayerInput()
    {
        if (_fireModeInputActions[FireMode.SingleFire]())
        {
            return true;  
        }

        if (_fireModeInputActions[FireMode.AutomaticFire]())
        {
            return true;  
        }

        if (_fireModeInputActions[FireMode.StopFiring]())
        {
            return false; 
        }

        return false; 
    }
}



