using UnityEngine;

[System.Serializable]
public class DebuffApplicationConfig
{
    [Range(0f, 1f)] public float Chance;
    public EffectDataSO Debuff;
    public int DurationOverride;
}

[CreateAssetMenu(fileName = "Behavior_", menuName = "SlotGame/Enemy/EnemyBehavior")]
public class EnemyBehaviorSO : ScriptableObject
{
    public string BehaviorName;
    public BehaviorType Type;

    [Header("Debuff Application (Attack behaviors)")]
    public DebuffApplicationConfig[] DebuffsOnHit;

    [Header("Buff Application (Support behaviors)")]
    public EffectDataSO BuffToApply;
    public int BuffDurationOverride;

    [Header("Summon (Summoner behaviors)")]
    public EnemyData EnemyToSummon;
}