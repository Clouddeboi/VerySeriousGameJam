using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("---Audio Sources---")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("---Music Clips---")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("---SFX Clips---")]
    [SerializeField] private AudioClip slotReelSpin;
    [SerializeField] private AudioClip slotResultShown;
    [SerializeField] private AudioClip playerAttack;
    [SerializeField] private AudioClip enemyAttack;
    [SerializeField] private AudioClip playerHurt;
    [SerializeField] private AudioClip enemyHurt;
    [SerializeField] private AudioClip buyItem;
    [SerializeField] private AudioClip enterRoom;
    [SerializeField] private AudioClip exitRoom;
    [SerializeField] private AudioClip enemyWheelSpin;
    [SerializeField] private AudioClip enemyWheelResult;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

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

    public void PlaySlotReelSpin() => PlaySFX(slotReelSpin);
    public void PlaySlotResultShown() => PlaySFX(slotResultShown);
    public void PlayPlayerAttack() => PlaySFX(playerAttack);
    public void PlayEnemyAttack() => PlaySFX(enemyAttack);
    public void PlayPlayerHurt() => PlaySFX(playerHurt);
    public void PlayEnemyHurt() => PlaySFX(enemyHurt);
    public void PlayBuyItem() => PlaySFX(buyItem);
    public void PlayEnterRoom() => PlaySFX(enterRoom);
    public void PlayExitRoom() => PlaySFX(exitRoom);
    public void PlayEnemyWheelSpin() => PlaySFX(enemyWheelSpin);
    public void PlayEnemyWheelResult() => PlaySFX(enemyWheelResult);
}