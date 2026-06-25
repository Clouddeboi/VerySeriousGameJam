using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_", menuName = "SlotGame/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string EnemyName;
    public int MaxHP;
    public int AttackDamage;
    public int HealAmount = 5;
    public Sprite Portrait;

    [Header("Action Weights")]
    public float AttackWeight = 70f;
    public float MissWeight = 10f;
    public float HealWeight = 20f;
}