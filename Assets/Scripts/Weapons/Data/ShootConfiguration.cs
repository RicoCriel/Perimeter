using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FIREMODE
{
    SINGLE,
    BURST, 
    AUTO
}

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Weapons/Shoot Configuration", order = 2)]
public class ShootConfiguration : ScriptableObject
{
    public FIREMODE Firemode;
    public LayerMask HitMask;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float FireRate = 0.25f;
    public bool IsSingleFire;
    public bool IsAutomaticFire;
    public float AimAssistStrength = 1.0f;
    public float MaxDeviationAngle = 30f;

    public float Recoil;
    public float RecoilSpeed; 
    public float RecoilReturnSpeed;
}
