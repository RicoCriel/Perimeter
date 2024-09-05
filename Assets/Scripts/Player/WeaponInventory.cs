using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    private Weapon currentWeapon;  // Reference to the currently equipped weapon
    private Weapon[] weapons;      // Array of all weapons attached to the player

    private void Start()
    {
        // Find all weapon objects attached to the player (initially inactive)
        weapons = GetComponentsInChildren<Weapon>(true);
        EquipWeapon(Weapon.WeaponType.Rifle);  // Start with rifle
    }

    public void EquipWeapon(Weapon.WeaponType weaponType)
    {
        // Deactivate the current weapon if there is one
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        // Find the new weapon to equip based on the weapon type
        foreach (var weapon in weapons)
        {
            if (weapon.weaponType == weaponType)
            {
                currentWeapon = weapon;
                currentWeapon.gameObject.SetActive(true);
                return;
            }
        }
    }
}
