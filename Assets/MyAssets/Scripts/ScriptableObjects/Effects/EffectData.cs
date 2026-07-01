using UnityEngine;

[System.Serializable]
public class EffectInteractionRule
{
    public EffectDataSO TargetEffect;
    public EffectInteractionType InteractionType;
}

[CreateAssetMenu(fileName = "Effect_", menuName = "SlotGame/Effects/EffectData")]
public class EffectDataSO : ScriptableObject
{
    [Header("Identity")]
    public string EffectId;
    public string DisplayName;
    public Sprite Icon;
    public EffectCategory Category;
    public StackBehavior Stacking = StackBehavior.StackDuration;

    [Header("Duration (0 = instant/interaction-only, never persists)")]
    public int DurationTurns = 1;

    [Header("Per-Turn Tick")]
    public bool TicksDamage = false;
    public int TickDamageAmount = 0;

    [Header("Per-Turn Heal (exp. Regeneration)")]
    public int HealPerTurn = 0;

    [Header("Action Prevention (exp. Freeze)")]
    public bool PreventsAction = false;

    [Header("Incoming Damage Block (exp. Shield)")]
    public bool BlocksIncomingDamage = false;
    public bool OnlyBlocksEnemyAttacks = true;

    [Header("Outgoing Damage Multiplier (exp. Strength)")]
    public float OutgoingDamageMultiplier = 1f;

    [Header("Interactions: resolved immediately on application")]
    public EffectInteractionRule[] Interactions;
}