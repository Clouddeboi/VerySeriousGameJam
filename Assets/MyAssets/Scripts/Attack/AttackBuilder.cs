using UnityEngine;

public class AttackBuilder : MonoBehaviour
{
    public Attack Build(SlotResult result, ResolvedSynergies synergies)
    {
        int baseDamage = result.Weapon ? result.Weapon.BaseDamage : 1;
        int hitCount = result.Weapon ? result.Weapon.BaseHitCount : 1;
        bool isCrit = false;
        int selfDamage = 0;
        EffectDataSO selfAppliedEffect = null;

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

            if (mod.HitCountOverride > 0)
                hitCount = mod.HitCountOverride;

            if (mod.HitDamageMultiplier != 1f)
                baseDamage = Mathf.RoundToInt(baseDamage * mod.HitDamageMultiplier);

            if (mod.SelfDamagePercent > 0f)
                selfDamage = Mathf.RoundToInt(baseDamage * mod.SelfDamagePercent);

            selfAppliedEffect = mod.SelfAppliedEffect;
        }

        //Apply pre-hit synergy bonuses on top of modifier behavior
        baseDamage += synergies.BonusFlatDamage;
        baseDamage = Mathf.RoundToInt(baseDamage * (1f + synergies.BonusPercentDamage));
        hitCount += synergies.BonusHits;
        hitCount = Mathf.Max(1, hitCount);

        var attack = new Attack
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

        Debug.Log($"[AttackBuilder] Built: {attack}");
        return attack;
    }
}