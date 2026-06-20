using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyData Data;
    public Health Health;

    public void Initialize(EnemyData data)
    {
        Data = data;
        Health.EntityName = data.EnemyName;
        Health.SetMaxHP(data.MaxHP);
    }

    //Always Attacks (TODO: add blocking, dodging etc.)
    public int DecideAndGetDamage()
    {
        Debug.Log($"[EnemyAI] {Data.EnemyName} attacks for {Data.AttackDamage}");
        return Data.AttackDamage;
    }
}