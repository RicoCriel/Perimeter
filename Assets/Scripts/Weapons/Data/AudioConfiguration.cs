using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfiguration", menuName = "Weapons/AudioConfiguration", order = 4)]
public class AudioConfiguration : ScriptableObject
{
    public AudioSource AudioSourcePrefab;
    public AudioClip WeaponShooting;
    public AudioClip WeaponEmpty;
    public AudioClip WeaponReloading;
}
