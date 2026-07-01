using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyData Data;
    public Health Health;
    public SpriteRenderer SpriteRenderer;
    public SquashStretchAnimator Animator;
    public HealthBarUI HealthBar;
    private CameraShake cameraShake;
    public StatusEffectVisual StatusVisual;
    public EffectGridUI EffectGrid;

    [Header("Damage Shake")]
    public float DamageShakeDuration = 0.15f;
    public float DamageShakeMagnitude = 0.12f;

    private bool deathHandled = false;

    public void Initialize(EnemyData data, CameraShake shake)
    {
        Data = data;
        cameraShake = shake;
        Health.EntityName = data.EnemyName;
        Health.SetMaxHP(data.MaxHP);

        if (SpriteRenderer != null && data.Portrait != null)
            SpriteRenderer.sprite = data.Portrait;

        Health.OnDeath -= HandleDeath;
        Health.OnDeath += HandleDeath;
        Health.OnDamaged -= HandleDamaged;
        Health.OnDamaged += HandleDamaged;

        HealthBar.Initialize(Health, transform);
        deathHandled = false;
    }

    public void InitializeStatusVisual(BuffDebuffSystem buffDebuff)
    {
        if (StatusVisual != null)
        {
            StatusVisual.BuffDebuff = buffDebuff;
            StatusVisual.Target = Health;
        }

        if (EffectGrid != null)
            EffectGrid.Initialize(buffDebuff, Health, transform); // new
    }

    private void HandleDamaged()
    {
        Animator.PlayDamageReaction();
        cameraShake?.Shake(DamageShakeDuration, DamageShakeMagnitude);
        AudioManager.Instance.PlayEnemyHurt();
    }

    private void HandleDeath()
    {
        if (deathHandled) return;
        deathHandled = true;

        if (SpriteRenderer != null) SpriteRenderer.enabled = false;
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        Destroy(gameObject);
    }

    public void PlayAttackAnimationOnly() => Animator.PlayAttackReaction();
}