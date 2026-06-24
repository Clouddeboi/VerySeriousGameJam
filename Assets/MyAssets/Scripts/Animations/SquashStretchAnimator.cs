using UnityEngine;
using System.Collections;

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
        if (!PlayIdle || reactionRoutine != null) return;

        idleTimer += Time.deltaTime * IdleSpeed;
        float stretchY = 1f + (Mathf.Sin(idleTimer) * 0.5f + 0.5f) * (IdleStretchAmount - 1f);

        float stretchX = 1f - (stretchY - 1f) * 0.5f;

        transform.localScale = new Vector3(baseScale.x * stretchX, baseScale.y * stretchY, baseScale.z);
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

    private IEnumerator ReactionRoutine(float targetY, float duration, System.Action onComplete)
    {
        float t = 0f;
        while (t < duration * 0.5f)
        {
            float progress = t / (duration * 0.5f);
            float y = Mathf.Lerp(1f, targetY, progress);
            float x = 1f - (y - 1f) * 0.5f; // inverse X for squash/stretch volume feel
            transform.localScale = new Vector3(baseScale.x * x, baseScale.y * y, baseScale.z);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < duration * 0.5f)
        {
            float progress = t / (duration * 0.5f);
            float y = Mathf.Lerp(targetY, 1f, progress);
            float x = 1f - (y - 1f) * 0.5f;
            transform.localScale = new Vector3(baseScale.x * x, baseScale.y * y, baseScale.z);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = baseScale;
        reactionRoutine = null;
        onComplete?.Invoke();
    }
}