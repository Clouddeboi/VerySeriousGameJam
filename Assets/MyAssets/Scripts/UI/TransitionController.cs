using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    public Image FadePanel;
    public float FadeDuration = 0.5f;
    private Coroutine activeTransition;

    private void Awake()
    {
        SetAlpha(0f);
        FadePanel.raycastTarget = false;
    }

    public void PlayTransition(Action onMidpoint)
    {
        if (activeTransition != null)
        {
            Debug.LogWarning("[Transition] PlayTransition called while one was already running — ignoring duplicate call.");
            return;
        }
        activeTransition = StartCoroutine(TransitionRoutine(onMidpoint));
    }

    private IEnumerator TransitionRoutine(Action onMidpoint)
    {
        yield return Fade(0f, 1f);
        onMidpoint?.Invoke();
        yield return null;
        yield return null;
        yield return Fade(1f, 0f);
        activeTransition = null;
    }

    private IEnumerator Fade(float from, float to)
    {
        FadePanel.raycastTarget = true;
        float t = 0f;
        while (t < FadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, t / FadeDuration));
            yield return null;
        }
        SetAlpha(to);
        FadePanel.raycastTarget = to > 0.99f;
    }

    private void SetAlpha(float a)
    {
        Color c = FadePanel.color;
        c.a = a;
        FadePanel.color = c;
    }

    public void PlayTransitionOneWay(Action onComplete)
    {
        if (activeTransition != null)
        {
            Debug.LogWarning("[Transition] PlayTransitionOneWay called while one was already running — ignoring duplicate call.");
            return;
        }
        activeTransition = StartCoroutine(OneWayRoutine(onComplete));
    }

    private IEnumerator OneWayRoutine(Action onComplete)
    {
        yield return Fade(0f, 1f);
        activeTransition = null; 
        onComplete?.Invoke();
    }

    public void FadeToClearOnly(System.Action onComplete)
    {
        StartCoroutine(FadeToClearRoutine(onComplete));
    }

    private IEnumerator FadeToClearRoutine(System.Action onComplete)
    {
        yield return Fade(1f, 0f);
        try { onComplete?.Invoke(); }
        catch (System.Exception e) { Debug.LogError($"[Transition] Exception in FadeToClearOnly onComplete: {e}"); }
    }
}