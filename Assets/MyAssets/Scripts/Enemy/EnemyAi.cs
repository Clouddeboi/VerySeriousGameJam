using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyData Data;
    public Health Health;
    public SpriteRenderer SpriteRenderer;
    public SquashStretchAnimator Animator;
    public HealthBarUI HealthBar;

    [Header("Damage Shake")]
    public float DamageShakeDuration = 0.15f;
    public float DamageShakeMagnitude = 0.12f;

    private bool deathHandled = false;

    public void Initialize(EnemyData data)
    {
        Data = data;
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

    private void HandleDamaged()
    {
        Animator.PlayDamageReaction();
        CameraShake.Instance.Shake(DamageShakeDuration, DamageShakeMagnitude);
        AudioManager.Instance.PlayEnemyHurt();
    }

    private void HandleDeath()
    {
        if (deathHandled) return;
        deathHandled = true;

        Debug.Log($"[EnemyAI] Removing {Data.EnemyName} sprite.");
        if (SpriteRenderer != null) SpriteRenderer.enabled = false;
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        Destroy(gameObject);
    }

    // public int DecideAndGetDamage()
    // {
    //     Debug.Log($"[EnemyAI] {Data.EnemyName} attacks for {Data.AttackDamage}");
    //     Animator.PlayAttackReaction();
    //     return Data.AttackDamage;
    // }

    public void PlayAttackAnimationOnly()
    {
        Animator.PlayAttackReaction();
    }
}