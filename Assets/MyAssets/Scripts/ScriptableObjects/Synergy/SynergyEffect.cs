using UnityEngine;

[CreateAssetMenu(fileName = "SynergyEffect_", menuName = "SlotGame/Synergy/SynergyEffect")]
public class SynergyEffect : ScriptableObject
{
    [Header("Editor Description")]
    public string Description;

    public SynergyEffectType Type;

    [Header("Damage")]
    public int FlatDamageBonus;
    public float PercentDamageBonus;
    public int AdditionalHits;

    [Header("Effect Application")]
    public EffectDataSO EffectData;
    public int DurationOverride;

    [Header("Heal")]
    public int HealAmount;

    [Header("Targeting")]
    public bool AffectsAllEnemies;
}