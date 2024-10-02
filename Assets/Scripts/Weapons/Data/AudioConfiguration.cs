using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfiguration", menuName = "Weapons/AudioConfiguration", order = 4)]
public class AudioConfiguration : ScriptableObject
{
    [Range(0f, 1f)]
    public float Volume = 1.0f;
    public AudioClip[] FireClips;
    public AudioClip EmptyClip;
    public AudioClip ReloadClip;
    public AudioClip LastBulletClip;

    public void PlayShootingClip(AudioSource AudioSource, bool IsLastBullet)
    {
        if(IsLastBullet && LastBulletClip != null)
        { 
            AudioSource.PlayOneShot(LastBulletClip, Volume);
        }
        else
        {
            AudioSource.PlayOneShot(FireClips[Random.Range(0, FireClips.Length)], Volume);
        }
    }

    public void PlayOutOfAmmoClip(AudioSource AudioSource)
    {
        if(EmptyClip != null)
        {
            AudioSource.PlayOneShot(EmptyClip, Volume);
        }
    }

    public void PlayReloadingClip(AudioSource AudioSource)
    {
        if(ReloadClip != null)
        {
            AudioSource.PlayOneShot(ReloadClip, Volume);
        }
    }


}
