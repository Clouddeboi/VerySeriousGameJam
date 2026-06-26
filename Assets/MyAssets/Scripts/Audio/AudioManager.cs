using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---Audio Sources---")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("---Music Clips---")]
    [SerializeField] private AudioClip backgroundMusic;

    // [Header("---SFX Clips---")]
    // [SerializeField] private AudioClip FootSteps;

    private void Start()
    {
        //Play background music
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    //Play a specific music clip
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    //Play a specific SFX clip
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }
}