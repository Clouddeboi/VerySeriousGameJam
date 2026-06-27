using UnityEngine;
using UnityEngine.UI;

public class ButtonHoverManager : MonoBehaviour
{
    [Header("Hover Settings")]
    public float hoverScale = 1.1f;
    public float scaleSpeed = 10f;

    [Header("Audio")]
    public AudioClip hoverSound;

    private AudioSource audioSource;

    [System.Obsolete]
    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            ButtonHover hover = button.gameObject.GetComponent<ButtonHover>();

            if (hover == null)
                hover = button.gameObject.AddComponent<ButtonHover>();

            hover.Initialize(hoverScale, scaleSpeed, audioSource, hoverSound);
        }
    }
}