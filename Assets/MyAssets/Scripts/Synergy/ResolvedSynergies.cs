using System.Collections.Generic;

public class ResolvedSynergies
{
    public int BonusFlatDamage = 0;
    public float BonusPercentDamage = 0f;
    public int BonusHits = 0;
    public int HealSelfAmount = 0;
    public bool AffectsAllEnemies = false;
    public List<SynergyEffect> PostHitEffects = new List<SynergyEffect>();
    public List<WeaponSynergy> MatchedSynergies = new List<WeaponSynergy>();
}