using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    private Weapon currentWeapon;  // Reference to the currently equipped weapon
    private Weapon[] weapons;      // Array of all weapons attached to the player
    public WeaponConfiguration ActiveWeaponConfig;
    public ParticleSystem ActiveWeaponParticleSystem;

    public static WeaponInventory Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    private void Start()
    {
        // Find all weapon objects attached to the player (initially inactive)
        weapons = GetComponentsInChildren<Weapon>(true);
        EquipWeaponObject(Weapon.WeaponType.Rifle);  // Start with rifle
    }

    public void EquipWeaponObject(Weapon.WeaponType weaponType)
    {
        // Deactivate the current weapon if there is one
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        // Find the new weapon to equip based on the weapon type
        foreach (var weapon in weapons)
        {
            if (weapon.Type == weaponType)
            {
                currentWeapon = weapon;
                currentWeapon.gameObject.SetActive(true);
                EquipActiveWeaponConfiguration(currentWeapon.Type);
                GetActiveWeaponParticleSystem(currentWeapon);
                return;
            }
        }
    }

    public void EquipActiveWeaponConfiguration(Weapon.WeaponType weaponType)
    {
        Weapon weapon = GetWeapon(weaponType);
        if (weapon != null)
        {
            ActiveWeaponConfig = weapon.Configuration;
            ActiveWeaponConfig.ActivateTrail(this);  // Activate the trail for the current weapon
            Debug.Log($"Trail Ready: {ActiveWeaponConfig} ");
        }
        else
        {
            Debug.LogError($"Weapon of type {weaponType} not found in inventory.");
        }
    }

    public Weapon GetWeapon(Weapon.WeaponType weaponType)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.Type == weaponType)
            {
                return weapon;
            }
        }
        return null;
    }

    public WeaponConfiguration GetActiveWeaponConfig()
    {
        return ActiveWeaponConfig;
    }

    public ParticleSystem GetActiveWeaponParticleSystem(Weapon currentWeapon)
    {
        ActiveWeaponParticleSystem = currentWeapon.gameObject.GetComponentInChildren<ParticleSystem>();
        return ActiveWeaponParticleSystem;
    }
}
