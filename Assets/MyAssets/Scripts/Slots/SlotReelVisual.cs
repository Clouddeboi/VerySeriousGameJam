using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlotReelVisual : MonoBehaviour
{
    public Image IconImage;
    public Sprite[] SpinSprites;
    public float SpinDuration = 0.6f;
    public float SpinFrameRate = 0.05f;

    public void PlaySpin(Sprite finalSprite, System.Action onComplete = null)
    {
        StopAllCoroutines();//Guard against a reset happening mid-spin
        StartCoroutine(SpinRoutine(finalSprite, onComplete));
    }

    public void SetIdle(Sprite idleSprite)
    {
        StopAllCoroutines();//Stop any in-flight spin so it doesn't override the idle sprite a frame later
        IconImage.sprite = idleSprite;
    }

    private IEnumerator SpinRoutine(Sprite finalSprite, System.Action onComplete)
    {
        float t = 0f;
        while (t < SpinDuration)
        {
            if (SpinSprites.Length > 0)
                IconImage.sprite = SpinSprites[Random.Range(0, SpinSprites.Length)];
            yield return new WaitForSeconds(SpinFrameRate);
            t += SpinFrameRate;
        }
        IconImage.sprite = finalSprite;
        onComplete?.Invoke();
    }
}