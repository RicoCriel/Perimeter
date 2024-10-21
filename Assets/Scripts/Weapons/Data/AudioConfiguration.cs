using UnityEngine;

public class AudioConfiguration : ScriptableObject
{
    [Range(0f, 1f)]
    public float Volume = 1.0f;
    public AudioClip FireClip;
    public AudioClip EmptyClip;
    public AudioClip ReloadClip;
    public AudioClip LastBulletClip;
    
    public void PlayShootingClip(AudioSource audioSource, bool isLastBullet, bool isAutomaticFire)
    {
        if (isAutomaticFire)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = FireClip;
                audioSource.loop = true; 
                audioSource.volume = Volume;
                audioSource.Play();
            }
        }
        else
        {
            if (isLastBullet && LastBulletClip != null)
            {
                audioSource.PlayOneShot(LastBulletClip, Volume);
            }
            else
            {
                audioSource.PlayOneShot(FireClip, Volume);
            }
        }
    }

    public void PlayOutOfAmmoClip(AudioSource audioSource)
    {
        if (EmptyClip != null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(EmptyClip, Volume);
            }
        }
    }

    public void PlayReloadingClip(AudioSource audioSource)
    {
        if (ReloadClip != null)
        {
            audioSource.PlayOneShot(ReloadClip, Volume);
        }
    }

    public void StopAudio(AudioSource audioSource)
    {
        if (audioSource.isPlaying)
        {
            audioSource.loop = false; 
            audioSource.Stop();      
        }
    }
}
