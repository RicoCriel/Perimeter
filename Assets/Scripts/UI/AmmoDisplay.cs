using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ammoClipText;
    [SerializeField] private TextMeshProUGUI _totalAmmoText;

    private void Update()
    {
        DisplayCurrentAmmo();
    }

    private void DisplayCurrentAmmo()
    {
        var activeWeaponConfig = WeaponInventory.Instance.ActiveWeaponConfig;
        _ammoClipText.SetText(activeWeaponConfig.AmmoConfig.ClipAmmo.ToString());
        _totalAmmoText.SetText(activeWeaponConfig.AmmoConfig.AvailableAmmo.ToString());
    }
}
