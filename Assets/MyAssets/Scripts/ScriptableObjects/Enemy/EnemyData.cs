using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_", menuName = "SlotGame/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string EnemyName;
    public Sprite Portrait;

    [Header("Types / Tags (can have multiple)")]
    public EnemyType[] Types;

    [Header("Faction")]
    public EnemyFaction Faction;

    [Header("Role")]
    public EnemyRoleSO Role;

    [Header("Base Stats")]
    public int BaseMaxHP = 20;
    public int BaseAttackDamage = 3;
    public int HealAmount = 5;

    [Header("Action Wheel Weights")]
    public float AttackWeight = 70f;
    public float MissWeight = 10f;
    public float HealWeight = 10f;
    public float SupportWeight = 10f;

    [Header("Behaviors (defines what actions actually do)")]
    public EnemyBehaviorSO AttackBehavior;
    public EnemyBehaviorSO HealBehavior;
    public EnemyBehaviorSO SupportBehavior;

    [Header("Elemental Affinities (overrides type defaults)")]
    public ElementalAffinityEntry[] AffinityOverrides;

    //Runtime stats after role multipliers apply
    public int MaxHP => Mathf.RoundToInt(BaseMaxHP * (Role != null ? Role.HPMultiplier : 1f));
    public int AttackDamage => Mathf.RoundToInt(BaseAttackDamage * (Role != null ? Role.DamageMultiplier : 1f));

    //Resolves effective action wheel weights, role overrides applied on top
    public void GetEffectiveWeights(out float attack, out float miss,
        out float heal, out float support)
    {
        attack = Role != null && Role.AttackWeightOverride > 0
            ? Role.AttackWeightOverride : AttackWeight;
        miss = Role != null && Role.MissWeightOverride > 0
            ? Role.MissWeightOverride : MissWeight;
        heal = Role != null && Role.HealWeightOverride > 0
            ? Role.HealWeightOverride : HealWeight;
        support = Role != null && Role.SupportWeightOverride > 0
            ? Role.SupportWeightOverride : SupportWeight;
    }

    //Resolves elemental response: individual override wins over type default
    public ElementalResponse GetElementalResponse(ElementType element)
    {
        //Check individual overrides first
        foreach (var entry in AffinityOverrides)
            if (entry.Element == element) return entry.Response;

        //Fall back to type defaults (first type that defines it wins)
        foreach (var type in Types)
            if (type != null)
                foreach (var entry in type.DefaultAffinities)
                    if (entry.Element == element) return entry.Response;

        return ElementalResponse.Normal;
    }

    public bool HasType(EnemyType type)
    {
        foreach (var t in Types)
            if (t == type) return true;
        return false;
    }
}