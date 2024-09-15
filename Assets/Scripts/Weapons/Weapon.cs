using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Shotgun,
        Rifle,
        GrenadeLauncher,
        Minigun,
        SubmachineGun
    }

    public WeaponType Type;
    public WeaponConfiguration Configuration;
}


