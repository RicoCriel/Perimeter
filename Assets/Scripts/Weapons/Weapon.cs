using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Shotgun,
        Rifle,
        Flamethrower,
        Minigun,
        SubmachineGun
    }

    public WeaponType Type;
    public WeaponConfiguration Configuration;
}


