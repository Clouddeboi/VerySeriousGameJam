using UnityEngine;

public static class AttackBuilder
{
    public static Attack Build(SlotResult result, ResolvedSynergies synergies,
        PassiveBonuses passiveBonuses = null, Character character = null)
    {
        int baseDamage = result.Weapon ? result.Weapon.BaseDamage : 1;
        int hitCount = result.Weapon ? result.Weapon.BaseHitCount : 1;
        bool isCrit = false;
        EffectDataSO selfAppliedEffect = null;
        int selfDamage = 0;

        if (result.Modifier != null)
        {
            var mod = result.Modifier;
            if (mod.OverrideDamage > 0)
            {
                baseDamage = mod.OverrideDamage;
            }
            else
            {
                baseDamage = Mathf.RoundToInt(baseDamage * mod.DamageMultiplier);
                if (mod.IsCrit)
                {
                    isCrit = true;
                    baseDamage = Mathf.RoundToInt(baseDamage * mod.CritMultiplier);
                }
            }

            if (mod.HitCountOverride > 0) hitCount = mod.HitCountOverride;
            if (mod.HitDamageMultiplier != 1f)
                baseDamage = Mathf.RoundToInt(baseDamage * mod.HitDamageMultiplier);
            if (mod.SelfDamagePercent > 0f)
                selfDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * mod.SelfDamagePercent));

            selfAppliedEffect = mod.SelfAppliedEffect;
        }

        if (character != null && character.DamageMultiplier != 1f)
            baseDamage = Mathf.RoundToInt(baseDamage * character.DamageMultiplier);

        if (character != null)
        {
            float bonus = RunState.Current.GetCharacter(character).DamageBonusPercent;
            if (bonus != 0f) baseDamage = Mathf.RoundToInt(baseDamage * (1f + bonus));
        }

        baseDamage += synergies.BonusFlatDamage;
        baseDamage = Mathf.RoundToInt(baseDamage * (1f + synergies.BonusPercentDamage));
        hitCount += synergies.BonusHits;

        if (passiveBonuses != null)
        {
            baseDamage += passiveBonuses.FlatDamageBonus;
            baseDamage = Mathf.RoundToInt(
                baseDamage * (1f + passiveBonuses.PercentDamageBonus));
        }

        hitCount = Mathf.Max(1, hitCount);

        return new Attack
        {
            BaseDamage = baseDamage,
            HitCount = hitCount,
            Element = result.Element,
            Weapon = result.Weapon,
            IsCrit = isCrit,
            Source = null,
            SelfDamageAmount = selfDamage,
            SelfAppliedEffect = selfAppliedEffect
        };
    }
}