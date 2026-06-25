using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyActionWheelVisual : MonoBehaviour
{
    [Header("Pie Chart Slices — ALL must be: Image Type=Filled, Fill Method=Radial 360, Fill Origin=Top, Clockwise=ON")]
    public Image AttackSlice;
    public Image MissSlice;
    public Image HealSlice;

    [Header("Hand — arrow art must point straight UP at local rotation 0")]
    public RectTransform Hand;

    [Header("Spin Animation")]
    public float SpinDuration = 1.0f;
    public int FullSpins = 3;
    public float LandEnlargeScale = 1.3f;
    public float LandEnlargeDuration = 0.2f;
    public float HoldAfterLandDuration = 0.6f;

    private static float ClockwiseAngleToZRotation(float angle) => -angle;

    public void Setup(EnemyData data)
    {
        var slices = EnemyActionWheel.BuildSlices(data);

        foreach (var slice in slices)
        {
            Image targetImage = slice.Action switch
            {
                EnemyActionType.Attack => AttackSlice,
                EnemyActionType.Miss => MissSlice,
                EnemyActionType.Heal => HealSlice,
                _ => null
            };
            if (targetImage == null) continue;

            float sliceFraction = (slice.EndAngle - slice.StartAngle) / 360f;
            targetImage.fillAmount = sliceFraction;
            targetImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, ClockwiseAngleToZRotation(slice.StartAngle));
        }
    }

    public void SpinAndLand(float landingAngle, System.Action onComplete)
    {
        gameObject.SetActive(true);
        StartCoroutine(SpinRoutine(landingAngle, onComplete));
    }

    private IEnumerator SpinRoutine(float landingAngle, System.Action onComplete)
    {
        Hand.localScale = Vector3.one;

        float finalTargetAngle = ClockwiseAngleToZRotation(landingAngle);
        Hand.localRotation = Quaternion.identity;
        float totalDistance = (360f * FullSpins) + ((360f - landingAngle) % 360f == 0 ? 0 : (360f - (landingAngle % 360f)));
        totalDistance = (360f * FullSpins) + landingAngle;

        float t = 0f;
        while (t < SpinDuration)
        {
            float progress = t / SpinDuration;
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            float clockwiseDegreesSoFar = Mathf.Lerp(0f, totalDistance, eased);
            Hand.localRotation = Quaternion.Euler(0f, 0f, ClockwiseAngleToZRotation(clockwiseDegreesSoFar));
            t += Time.deltaTime;
            yield return null;
        }

        Hand.localRotation = Quaternion.Euler(0f, 0f, finalTargetAngle);

        t = 0f;
        while (t < LandEnlargeDuration * 0.5f)
        {
            float scale = Mathf.Lerp(1f, LandEnlargeScale, t / (LandEnlargeDuration * 0.5f));
            transform.localScale = Vector3.one * scale;
            t += Time.deltaTime;
            yield return null;
        }
        t = 0f;
        while (t < LandEnlargeDuration * 0.5f)
        {
            float scale = Mathf.Lerp(LandEnlargeScale, 1f, t / (LandEnlargeDuration * 0.5f));
            transform.localScale = Vector3.one * scale;
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;

        yield return new WaitForSeconds(HoldAfterLandDuration);

        gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    public void DebugTestAngle(float angle)
    {
        Debug.Log($"[WheelTest] Testing angle {angle}");
        SpinAndLand(angle, () => Debug.Log("[WheelTest] Landed"));
    }
}