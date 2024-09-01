using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float ShootingRange { get; protected set; }  
    public float ShootingFrequency { get; protected set; }  
    public float ReloadSpeed { get; protected set; }

    public abstract void Shoot();
    public abstract void Reload();

    public virtual void Equip()
    {
        Debug.Log("Weapon equipped.");
    }
}
