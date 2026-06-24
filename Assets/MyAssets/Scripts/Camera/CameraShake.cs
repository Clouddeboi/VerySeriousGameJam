using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    public Transform CameraTransform;
    private Vector3 originalLocalPos;
    private Coroutine activeShake;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (CameraTransform == null) CameraTransform = transform;
        originalLocalPos = CameraTransform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (activeShake != null) StopCoroutine(activeShake);
        activeShake = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float t = 0f;
        while (t < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            CameraTransform.localPosition = originalLocalPos + new Vector3(x, y, 0f);
            t += Time.deltaTime;
            yield return null;
        }
        CameraTransform.localPosition = originalLocalPos;
        activeShake = null;
    }
}