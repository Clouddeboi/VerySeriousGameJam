using UnityEngine;

public class PartyMember : MonoBehaviour
{
    public Character Data;
    public Health Health;
    public SpriteRenderer SpriteRenderer;
    public SquashStretchAnimator Animator;
    public HealthBarUI HealthBar;
    public EffectGridUI EffectGrid;

    private CameraShake cameraShake;

    private void Awake()
    {
        if (Health == null) Debug.LogError($"[PartyMember] {gameObject.name} missing Health component reference.");
        if (Animator == null) Debug.LogWarning($"[PartyMember] {gameObject.name} missing SquashStretchAnimator.");
        if (HealthBar == null) Debug.LogWarning($"[PartyMember] {gameObject.name} missing SpriteHealthBar.");
        if (EffectGrid == null) Debug.LogWarning($"[PartyMember] {gameObject.name} missing EffectGridUI.");
    }

    public void Initialize(Character data, CameraShake shake,
        BuffDebuffSystem buffDebuff, FloatingTextSpawner textSpawner)
    {
        Data = data;
        cameraShake = shake;

        Health.EntityName = data.CharacterName;
        Health.SetMaxHP(data.MaxHP);
        Health.TextSpawner = textSpawner;

        if (SpriteRenderer != null && data.Portrait != null)
            SpriteRenderer.sprite = data.Portrait;

        Health.OnDamaged += HandleDamaged;
        Health.OnDeath += HandleDeath;

        HealthBar?.Initialize(Health, transform);
        EffectGrid?.Initialize(buffDebuff, Health, transform);
    }

    private void HandleDamaged()
    {
        Animator?.PlayDamageReaction();
        cameraShake?.Shake(0.15f, 0.12f);
    }

    // The object stays alive so the party list and turn order keep working
    private void HandleDeath()
    {
        if (SpriteRenderer != null) SpriteRenderer.enabled = false;
        if (HealthBar != null && HealthBar.gameObject != gameObject) HealthBar.gameObject.SetActive(false);
        if (EffectGrid != null && EffectGrid.gameObject != gameObject) EffectGrid.gameObject.SetActive(false);
    }

    public void PlayAttackAnimation() => Animator?.PlayAttackReaction();

    private void OnDestroy()
    {
        if (Health != null)
        {
            Health.OnDamaged -= HandleDamaged;
            Health.OnDeath -= HandleDeath;
        }
    }
}