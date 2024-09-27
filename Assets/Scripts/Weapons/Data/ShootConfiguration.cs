using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Weapons/Shoot Configuration", order = 2)]
public class ShootConfiguration : ScriptableObject
{
    public LayerMask HitMask;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float FireRate = 0.25f;
    public bool IsSingleFire;
    public bool IsAutomaticFire;

    public float Recoil;
    public float RecoilSpeed; 
    public float RecoilReturnSpeed;
}
