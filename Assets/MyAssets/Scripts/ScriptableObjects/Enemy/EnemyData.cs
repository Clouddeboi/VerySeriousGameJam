using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_", menuName = "SlotGame/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string EnemyName;
    public int MaxHP;
    public int AttackDamage;
    public Sprite Portrait;
}