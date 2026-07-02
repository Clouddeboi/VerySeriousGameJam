using UnityEngine;

[CreateAssetMenu(fileName = "Modifier_", menuName = "SlotGame/Symbols/Modifier")]
public class ModifierSymbol : ScriptableObject
{
    public string DisplayName;
    public Sprite Icon;

    [Header("Base Damage Behavior")]
    public float DamageMultiplier = 1f;
    public bool IsCrit = false;
    public float CritMultiplier = 1.5f;
    public int OverrideDamage = 0;

    [Header("Hit Count Behavior")]
    public int HitCountOverride = 0;
    public float HitDamageMultiplier = 1f;

    [Header("Self Effects (applied once per attack, not per hit)")]
    public float SelfDamagePercent = 0f;
    public EffectDataSO SelfAppliedEffect;
}