using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TextMeshPro Text;

    [Header("Punch")]
    public float PunchScale = 1.4f;
    public float PunchDuration = 0.15f;

    [Header("Float & Fade")]
    public float FloatDistance = 1f;
    public float LifeDuration = 1f;

    [Header("Typewriter (optional)")]
    public float TypewriterCharDelay = 0.03f;

    private Vector3 startPos;

    public void Play(string content, Color color)
    {
        startPos = transform.position;
        Text.color = color;
        StartCoroutine(PlayRoutine(content));
    }

    private IEnumerator PlayRoutine(string content)
    {
        Text.text = "";
        transform.localScale = Vector3.zero;

        float t = 0f;
        while (t < PunchDuration)
        {
            float scale = Mathf.Lerp(0f, PunchScale, t / PunchDuration);
            transform.localScale = Vector3.one * scale;
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;

        if (TypewriterCharDelay > 0f)
        {
            for (int i = 0; i < content.Length; i++)
            {
                Text.text = content.Substring(0, i + 1);
                yield return new WaitForSeconds(TypewriterCharDelay);
            }
        }
        else
        {
            Text.text = content;
        }

        t = 0f;
        Color startColor = Text.color;
        while (t < LifeDuration)
        {
            float progress = t / LifeDuration;
            transform.position = startPos + Vector3.up * (FloatDistance * progress);

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, progress);
            Text.color = c;

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}