using UnityEngine;

public class PlayerCombatVisuals : MonoBehaviour
{
    public Health Health;
    public SquashStretchAnimator Animator;
    public HealthBarUI HealthBar;

    [Header("Damage Shake")]
    public float DamageShakeDuration = 0.15f;
    public float DamageShakeMagnitude = 0.12f;

    private void Awake()
    {
        Health.OnDamaged += HandleDamaged;
        HealthBar.Initialize(Health, transform);
    }

    private void HandleDamaged()
    {
        Animator.PlayDamageReaction();
        CameraShake.Instance.Shake(DamageShakeDuration, DamageShakeMagnitude);
    }

    public void PlayAttackAnimation()
    {
        Animator.PlayAttackReaction();
    }

    private void OnDestroy()
    {
        if (Health != null) Health.OnDamaged -= HandleDamaged;
    }
}