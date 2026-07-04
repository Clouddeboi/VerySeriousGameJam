using UnityEngine;

[CreateAssetMenu(fileName = "EnemyRole_", menuName = "SlotGame/Enemy/EnemyRole")]
public class EnemyRoleSO : ScriptableObject
{
    public string RoleName;
    public EnemyRole Role;

    [Header("Action Wheel Weight Overrides (0 = use enemy's base weight)")]
    public float AttackWeightOverride;
    public float MissWeightOverride;
    public float HealWeightOverride;
    public float SupportWeightOverride;

    [Header("Role Stat Modifiers")]
    public float HPMultiplier = 1f;
    public float DamageMultiplier = 1f;
}
