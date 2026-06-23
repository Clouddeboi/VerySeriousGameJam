using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlotReelVisual : MonoBehaviour
{
    public Image IconImage;
    public Sprite[] SpinSprites;
    public float SpinDuration = 0.6f;
    public float SpinFrameRate = 0.05f;

    [Header("Landing Impact")]
    public float ImpactScaleMultiplier = 1.4f;
    public float ImpactScaleDuration = 0.15f;
    public float ImpactShakeDuration = 0.2f;
    public float ImpactShakeMagnitude = 8f;

    private Vector3 originalScale;
    private Vector3 originalLocalPos;

    private void Awake()
    {
        originalScale = IconImage.transform.localScale;
        originalLocalPos = IconImage.transform.localPosition;
    }

    public void PlaySpin(Sprite finalSprite, System.Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(SpinRoutine(finalSprite, onComplete));
    }

    public void SetIdle(Sprite idleSprite)
    {
        StopAllCoroutines();
        IconImage.sprite = idleSprite;
        IconImage.transform.localScale = originalScale;
        IconImage.transform.localPosition = originalLocalPos;
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

        yield return StartCoroutine(LandingImpact());

        onComplete?.Invoke();
    }

    private IEnumerator LandingImpact()
    {
        //Scale punch, grow then settle back
        float t = 0f;
        while (t < ImpactScaleDuration)
        {
            float progress = t / ImpactScaleDuration;
            float scale = Mathf.Lerp(ImpactScaleMultiplier, 1f, progress);
            IconImage.transform.localScale = originalScale * scale;
            t += Time.deltaTime;
            yield return null;
        }
        IconImage.transform.localScale = originalScale;

        //Self-shake, runs concurrently-feeling since it's quick and right after the punch
        t = 0f;
        while (t < ImpactShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * ImpactShakeMagnitude;
            float y = Random.Range(-1f, 1f) * ImpactShakeMagnitude;
            IconImage.transform.localPosition = originalLocalPos + new Vector3(x, y, 0f);
            t += Time.deltaTime;
            yield return null;
        }
        IconImage.transform.localPosition = originalLocalPos;
    }
}