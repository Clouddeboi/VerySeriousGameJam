using UnityEngine;

[CreateAssetMenu(fileName = "PassiveEffect_", menuName = "SlotGame/Party/PassiveEffect")]
public class PassiveEffect : ScriptableObject
{
    public string Description;
    public PassiveEffectType Type;

    [Header("Buff / Debuff Application")]
    public EffectDataSO EffectData;
    public int DurationOverride;

    [Header("Damage Bonus")]
    public int FlatDamageBonus;
    public float PercentDamageBonus;

    [Header("Heal")]
    public int HealAmount;

    [Header("Additional Effect Stacks")]
    public int AdditionalStacks;
}