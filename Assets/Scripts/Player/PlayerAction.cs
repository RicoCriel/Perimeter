using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetMouseButton(0) && WeaponInventory.Instance.ActiveWeaponConfig != null)
        {
            WeaponInventory.Instance.ActiveWeaponConfig.Shoot(WeaponInventory.Instance.ActiveWeaponParticleSystem);
        }
    }
}
