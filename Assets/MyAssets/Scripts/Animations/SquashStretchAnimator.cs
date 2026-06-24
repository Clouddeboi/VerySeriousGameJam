using System.Collections;
using UnityEngine;

public class SquashStretchAnimator : MonoBehaviour
{
    [Header("Idle Bob")]
    public bool PlayIdle = true;
    public float IdleStretchAmount = 1.08f;
    public float IdleSpeed = 1.5f;

    [Header("Damage Reaction")]
    public float DamageShrinkAmount = 0.8f;
    public float DamageReactionDuration = 0.25f;

    [Header("Attack Reaction")]
    public float AttackEnlargeAmount = 1.25f;
    public float AttackReactionDuration = 0.2f;

    private Vector3 baseScale;
    private Coroutine reactionRoutine;
    private float idleTimer;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (!PlayIdle || reactionRoutine != null) return;//Pause idle while a reaction is playing

        idleTimer += Time.deltaTime * IdleSpeed;
        float stretch = 1f + (Mathf.Sin(idleTimer) * 0.5f + 0.5f) * (IdleStretchAmount - 1f);
        transform.localScale = new Vector3(baseScale.x, baseScale.y * stretch, baseScale.z);
    }

    public void PlayDamageReaction(System.Action onComplete = null)
    {
        if (reactionRoutine != null) StopCoroutine(reactionRoutine);
        reactionRoutine = StartCoroutine(ReactionRoutine(DamageShrinkAmount, DamageReactionDuration, onComplete));
    }

    public void PlayAttackReaction(System.Action onComplete = null)
    {
        if (reactionRoutine != null) StopCoroutine(reactionRoutine);
        reactionRoutine = StartCoroutine(ReactionRoutine(AttackEnlargeAmount, AttackReactionDuration, onComplete));
    }

    private IEnumerator ReactionRoutine(float targetScale, float duration, System.Action onComplete)
    {
        float t = 0f;

        //Punch to target scale
        while (t < duration * 0.5f)
        {
            float progress = t / (duration * 0.5f);
            float scale = Mathf.Lerp(1f, targetScale, progress);
            transform.localScale = baseScale * scale;
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        //Settle back to base
        while (t < duration * 0.5f)
        {
            float progress = t / (duration * 0.5f);
            float scale = Mathf.Lerp(targetScale, 1f, progress);
            transform.localScale = baseScale * scale;
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = baseScale;
        reactionRoutine = null;
        onComplete?.Invoke();
    }
}