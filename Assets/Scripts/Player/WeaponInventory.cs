using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    private Weapon _currentWeapon;  
    private Weapon[] _weapons;      
    public WeaponConfiguration ActiveWeaponConfig;
    public ParticleSystem ActiveWeaponShootSystem;

    public static WeaponInventory Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Start()
    {
        // Find all weapon objects attached to the player (initially inactive)
        _weapons = GetComponentsInChildren<Weapon>(true);
        EquipWeaponObject(Weapon.WeaponType.Rifle);  // Start with rifle
    }

    public void EquipWeaponObject(Weapon.WeaponType weaponType)
    {
        // Deactivate the current weapon if there is one
        if (_currentWeapon != null)
        {
            _currentWeapon.gameObject.SetActive(false);
        }

        // Find the new weapon to equip based on the weapon type
        foreach (var weapon in _weapons)
        {
            if (weapon.Type == weaponType)
            {
                _currentWeapon = weapon;
                _currentWeapon.gameObject.SetActive(true);
                EquipActiveWeaponConfiguration(_currentWeapon.Type);
                GetActiveWeaponParticleSystem(_currentWeapon);
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
            ActiveWeaponConfig.ActivateBulletTrail(this);  // Activate the trail for the current weapon
        }
        //else
        //{
        //    Debug.LogError($"Weapon of type {weaponType} not found in inventory.");
        //}
    }

    public Weapon GetWeapon(Weapon.WeaponType weaponType)
    {
        foreach (var weapon in _weapons)
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
        ActiveWeaponShootSystem = currentWeapon.gameObject.GetComponentInChildren<ParticleSystem>();
        return ActiveWeaponShootSystem;
    }
}
