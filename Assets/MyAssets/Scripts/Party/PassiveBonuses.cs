using System.Collections.Generic;

public class PassiveBonuses
{
    public int FlatDamageBonus = 0;
    public float PercentDamageBonus = 0f;
    public int AdditionalEffectStacks = 0;
    public List<EffectDataSO> BuffsToApplyToSelf = new List<EffectDataSO>();
    public List<EffectDataSO> DebuffsToApplyToTarget = new List<EffectDataSO>();
}