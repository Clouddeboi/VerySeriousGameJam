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

    [Header("Elemental Weakness (legacy single weakness, still honoured)")]
    public ElementType ElementalWeakness = ElementType.None;

    [Header("Elemental Affinities (damage received; overrides the weakness above)")]
    public ElementalAffinityEntry[] ElementalAffinities;

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

    // Explicit affinity entries win; otherwise the legacy single weakness applies
    public ElementalResponse GetElementalResponse(ElementType element)
    {
        if (ElementalAffinities != null)
            foreach (var entry in ElementalAffinities)
                if (entry.Element == element) return entry.Response;

        if (element != ElementType.None && element == ElementalWeakness)
            return ElementalResponse.Weak;

        return ElementalResponse.Normal;
    }

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