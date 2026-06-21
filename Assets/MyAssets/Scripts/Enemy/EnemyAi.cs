using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyData Data;
    public Health Health;
    public SpriteRenderer SpriteRenderer;

    public void Initialize(EnemyData data)
    {
        Data = data;
        Health.EntityName = data.EnemyName;
        Health.SetMaxHP(data.MaxHP);

        if (SpriteRenderer != null && data.Portrait != null)
            SpriteRenderer.sprite = data.Portrait;
    }

    private void HandleDeath()
    {
        Debug.Log($"[EnemyAI] Removing {Data.EnemyName} sprite.");
        Destroy(gameObject, 0.1f); 
    }

    //Always Attacks (TODO: add blocking, dodging etc.)
    public int DecideAndGetDamage()
    {
        Debug.Log($"[EnemyAI] {Data.EnemyName} attacks for {Data.AttackDamage}");
        return Data.AttackDamage;
    }
}