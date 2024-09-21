using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trail Config", menuName = "Weapons/WeaponTrail Config", order = 4)]
public class TrailConfiguration : ScriptableObject
{
    //Everything to do with the guns Bullet trail
    public Material Material;
    public AnimationCurve WidthCurve;
    public int MaxAmount;
    public float Duration = 0.5f;
    public float MinVertexDistance = 0.1f;
    public Gradient Color;

    public float MissDistance = 100f;
    public float SimulationSpeed = 100f;
}
