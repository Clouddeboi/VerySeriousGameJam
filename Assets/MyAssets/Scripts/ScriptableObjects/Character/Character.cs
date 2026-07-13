using UnityEngine;

[CreateAssetMenu(fileName = "Character_", menuName = "SlotGame/Party/Character")]
public class Character : ScriptableObject
{
    [Header("Identity")]
    public string CharacterName;
    [TextArea] public string Description;
    public Sprite Portrait;

    [Header("Role")]
    public CharacterRoleSO Role;

    [Header("Core Stats")]
    public int MaxHP = 50;
    public float DamageMultiplier = 1f;
    public float HealMultiplier = 1f;

    [Header("Elemental Weakness")]
    public ElementType ElementalWeakness = ElementType.None;

    [Header("Weapon Lock (optional — leave null for normal weapon reel)")]
    public WeaponSymbol WeaponLock;

    [Header("Reel Configurations (character-specific weights)")]
    public WeaponReelConfig WeaponReelConfig;
    public ElementReelConfig ElementReelConfig;
    public ModifierReelConfig ModifierReelConfig;

    [Header("Passive Abilities")]
    public PassiveSO[] Passives;

    [Header("Animations (all optional)")]
    public CharacterAnimationConfig Animations;

    public bool HasPassiveFor(PassiveTriggerType trigger)
    {
        foreach (var p in Passives)
            if (p != null && p.Trigger == trigger) return true;
        return false;
    }

    public PassiveSO[] GetPassivesFor(PassiveTriggerType trigger)
    {
        var result = new System.Collections.Generic.List<PassiveSO>();
        foreach (var p in Passives)
            if (p != null && p.Trigger == trigger) result.Add(p);
        return result.ToArray();
    }
}