using UnityEngine;

public class PlayerCombatVisuals : MonoBehaviour
{
    public Health Health;
    public SquashStretchAnimator Animator;
    public CameraShake CameraShake;
    public HealthBarUI HealthBar;
    public EffectGridUI EffectGrid;

    [Header("Damage Shake")]
    public float DamageShakeDuration = 0.15f;
    public float DamageShakeMagnitude = 0.12f;

    public void Initialize(BuffDebuffSystem buffDebuff)
    {
        HealthBar.Initialize(Health, transform);

        if (EffectGrid != null)
            EffectGrid.Initialize(buffDebuff, Health, transform);
    }

    private void Awake()
    {
        Health.OnDamaged += HandleDamaged;
    }

    private void HandleDamaged()
    {
        Animator.PlayDamageReaction();
        CameraShake.Shake(DamageShakeDuration, DamageShakeMagnitude);
        AudioManager.Instance.PlayPlayerHurt();
    }

    public void PlayAttackAnimation() => Animator.PlayAttackReaction();

    private void OnDestroy()
    {
        if (Health != null) Health.OnDamaged -= HandleDamaged;
    }
}