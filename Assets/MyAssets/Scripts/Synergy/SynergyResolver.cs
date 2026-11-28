using System.Collections.Generic;
using UnityEngine;

public class SynergyResolver : MonoBehaviour
{
    public WeaponSynergy[] AllSynergies;
    public BuffDebuffSystem BuffDebuff;
    //public Health PlayerHealth;

    public ResolvedSynergies Resolve(SlotResult result)
    {
        var resolved = new ResolvedSynergies();

        foreach (var synergy in AllSynergies)
        {
            if (synergy == null || !synergy.Matches(result)) continue;

            Debug.Log($"[Synergy] Triggered: {synergy.SynergyName}");
            resolved.MatchedSynergies.Add(synergy);

            foreach (var effect in synergy.Effects)
                CollectEffect(effect, resolved);
        }

        return resolved;
    }

    private void CollectEffect(SynergyEffect effect, ResolvedSynergies resolved)
    {
        switch (effect.Type)
        {
            case SynergyEffectType.FlatDamageBonus:
                resolved.BonusFlatDamage += effect.FlatDamageBonus;
                break;
            case SynergyEffectType.PercentDamageBonus:
                resolved.BonusPercentDamage += effect.PercentDamageBonus;
                break;
            case SynergyEffectType.AdditionalHits:
                resolved.BonusHits += effect.AdditionalHits;
                break;
            case SynergyEffectType.HealSelf:
                resolved.HealSelfAmount += effect.HealAmount;
                break;
            // Everything else deferred until after damage lands
            default:
                resolved.PostHitEffects.Add(effect);
                break;
        }

        if (effect.AffectsAllEnemies)
            resolved.AffectsAllEnemies = true;
    }

    public void ApplyPostHitEffects(ResolvedSynergies resolved, Health attacker,
        Health primaryTarget, List<EnemyAI> allEnemies)
    {
        foreach (var effect in resolved.PostHitEffects)
        {
            var targets = ResolveTargets(effect, primaryTarget, allEnemies);

            foreach (var t in targets)
            {
                switch (effect.Type)
                {
                    case SynergyEffectType.ApplyEffectToTarget:
                        if (effect.EffectData != null)
                        {
                            int dur = effect.DurationOverride > 0 ? effect.DurationOverride : -1;
                            BuffDebuff.ApplyEffect(t, effect.EffectData, dur);
                        }
                        break;
                    case SynergyEffectType.RemoveEffectFromTarget:
                        if (effect.EffectData != null)
                            BuffDebuff.RemoveEffect(t, effect.EffectData);
                        break;
                    case SynergyEffectType.HealTarget:
                        t.Heal(effect.HealAmount);
                        break;
                }
            }

            if (effect.Type == SynergyEffectType.ApplyEffectToSelf
                && effect.EffectData != null)
            {
                int dur = effect.DurationOverride > 0 ? effect.DurationOverride : -1;
                BuffDebuff.ApplyEffect(attacker, effect.EffectData, dur);
            }

            if (effect.Type == SynergyEffectType.RemoveEffectFromSelf
                && effect.EffectData != null)
                BuffDebuff.RemoveEffect(attacker, effect.EffectData);
        }

        if (resolved.HealSelfAmount > 0)
            attacker?.Heal(resolved.HealSelfAmount);
    }

    private List<Health> ResolveTargets(SynergyEffect effect,
        Health primary, List<EnemyAI> allEnemies)
    {
        var targets = new List<Health>();
        if (effect.AffectsAllEnemies)
        {
            foreach (var e in allEnemies)
                if (e != null && !e.Health.IsDead) targets.Add(e.Health);
        }
        else if (primary != null && !primary.IsDead)
        {
            targets.Add(primary);
        }
        return targets;
    }
}