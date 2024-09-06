using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBoxPurchase : MonoBehaviour
{
    private WeaponBox _weaponBox;

    private void Awake()
    {
        _weaponBox = GetComponentInParent<WeaponBox>();
    }
    public void EnableBuy()
    {
        _weaponBox.CanBuyWeapon = true;
    }

    public void DisableBuy()
    {
        _weaponBox.CanBuyWeapon = false;
    }
}
