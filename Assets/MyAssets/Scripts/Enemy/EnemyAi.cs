using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyData Data;
    public Health Health;
    public SpriteRenderer SpriteRenderer;
    private bool deathHandled = false;

    public void Initialize(EnemyData data)
    {
        Data = data;
        Health.EntityName = data.EnemyName;
        Health.SetMaxHP(data.MaxHP);

        if (SpriteRenderer != null && data.Portrait != null)
            SpriteRenderer.sprite = data.Portrait;

        Health.OnDeath -= HandleDeath; //Guard against double-subscribe
        Health.OnDeath += HandleDeath;
        deathHandled = false;
    }

    private void HandleDeath()
    {
        if (deathHandled) return;
        deathHandled = true;

        Debug.Log($"[EnemyAI] Removing {Data.EnemyName} sprite.");
        if (SpriteRenderer != null) SpriteRenderer.enabled = false; //Instant visual removal
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false; //Stop further clicks on a corpse

        Destroy(gameObject); //No delay, removes any timing ambiguity
    }

    public int DecideAndGetDamage()
    {
        Debug.Log($"[EnemyAI] {Data.EnemyName} attacks for {Data.AttackDamage}");
        return Data.AttackDamage;
    }
}