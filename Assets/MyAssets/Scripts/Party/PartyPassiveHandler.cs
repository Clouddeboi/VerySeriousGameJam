using System.Collections.Generic;
using UnityEngine;

public class PartyPassiveHandler : MonoBehaviour
{
    public PartyManager Party;
    public BuffDebuffSystem BuffDebuff;
    public FloatingTextSpawner TextSpawner;

    public void HandleCombatStart(List<EnemyAI> enemies)
    {
        foreach (var member in Party.ActiveMembers)
        {
            if (member == null || member.Health.IsDead) continue;
            foreach (var passive in member.Data.GetPassivesFor(PassiveTriggerType.OnCombatStart))
                ApplyPassiveEffects(passive, member, null, enemies);
        }
    }

    public PassiveBonuses EvaluateOffensivePassives(PartyMember attacker,
        EnemyAI target, ElementType element, bool isCrit)
    {
        var bonuses = new PassiveBonuses();
        if (attacker?.Data?.Passives == null) return bonuses;

        foreach (var passive in attacker.Data.Passives)
        {
            if (passive == null) continue;

            bool triggered = false;

            if (passive.Trigger == PassiveTriggerType.OnAttackingEnemyType
                && target != null)
            {
                foreach (var type in target.Data.Types)
                    if (passive.MatchesEnemyType(type)) { triggered = true; break; }
            }
            else if (passive.Trigger == PassiveTriggerType.OnElementUsed
                && passive.MatchesElement(element))
            {
                triggered = true;
            }
            else if (passive.Trigger == PassiveTriggerType.OnCriticalHit && isCrit)
            {
                triggered = true;
            }

            if (triggered)
                CollectPassiveBonuses(passive, bonuses);
        }

        return bonuses;
    }

    public void HandlePlayerAttacked(PartyMember defender,
        EnemyAI attacker, List<EnemyAI> allEnemies)
    {
        if (defender?.Data?.Passives == null) return;

        foreach (var passive in defender.Data.Passives)
        {
            if (passive == null
                || passive.Trigger != PassiveTriggerType.OnAttackedByEnemyType)
                continue;

            bool matched = false;
            foreach (var type in attacker.Data.Types)
                if (passive.MatchesEnemyType(type)) { matched = true; break; }

            if (matched)
                ApplyPassiveEffects(passive, defender, null, allEnemies);
        }
    }

    public bool IsImmuneToEnemyType(PartyMember member, EnemyType enemyType)
    {
        foreach (var passive in member.Data.Passives)
        {
            if (passive == null || !passive.IsImmunity) continue;
            if (passive.ImmuneToEnemyType == enemyType) return true;
        }
        return false;
    }

    public bool IsImmuneToElement(PartyMember member, ElementType element)
    {
        foreach (var passive in member.Data.Passives)
        {
            if (passive == null || !passive.IsImmunity) continue;
            if (passive.ImmuneToElement == element) return true;
        }
        return false;
    }

    public bool IsImmuneToEffect(PartyMember member, EffectDataSO effect)
    {
        foreach (var passive in member.Data.Passives)
        {
            if (passive == null || !passive.IsImmunity) continue;
            if (passive.ImmuneToEffect == effect) return true;
        }
        return false;
    }

    private void ApplyPassiveEffects(PassiveSO passive, PartyMember self,
        EnemyAI primaryTarget, List<EnemyAI> allEnemies)
    {
        foreach (var effect in passive.Effects)
        {
            if (effect == null) continue;
            ApplySinglePassiveEffect(effect, self, primaryTarget, allEnemies);
        }
    }

    private void ApplySinglePassiveEffect(PassiveEffect effect, PartyMember self,
        EnemyAI primaryTarget, List<EnemyAI> allEnemies)
    {
        int duration = effect.DurationOverride > 0 ? effect.DurationOverride : -1;

        switch (effect.Type)
        {
            case PassiveEffectType.ApplyBuffToSelf:
                if (effect.EffectData != null)
                    BuffDebuff.ApplyEffect(self.Health, effect.EffectData, duration);
                break;

            case PassiveEffectType.ApplyDebuffToAllEnemies:
                if (effect.EffectData != null && allEnemies != null)
                    foreach (var e in allEnemies)
                        if (e != null && !e.Health.IsDead)
                            BuffDebuff.ApplyEffect(e.Health, effect.EffectData, duration);
                break;

            case PassiveEffectType.ApplyDebuffToTarget:
                if (effect.EffectData != null && primaryTarget != null
                    && !primaryTarget.Health.IsDead)
                    BuffDebuff.ApplyEffect(primaryTarget.Health, effect.EffectData, duration);
                break;

            case PassiveEffectType.HealSelf:
                self.Health.Heal(effect.HealAmount);
                break;
        }

        TextSpawner?.SpawnEffectAppliedText(
            $"Passive: {effect.Description}",
            true,
            self.transform.position + Vector3.up * 2f);
    }

    private void CollectPassiveBonuses(PassiveSO passive, PassiveBonuses bonuses)
    {
        foreach (var effect in passive.Effects)
        {
            if (effect == null) continue;
            switch (effect.Type)
            {
                case PassiveEffectType.FlatDamageBonus:
                    bonuses.FlatDamageBonus += effect.FlatDamageBonus;
                    break;
                case PassiveEffectType.PercentDamageBonus:
                    bonuses.PercentDamageBonus += effect.PercentDamageBonus;
                    break;
                case PassiveEffectType.AdditionalEffectStacks:
                    bonuses.AdditionalEffectStacks += effect.AdditionalStacks;
                    break;
                case PassiveEffectType.ApplyBuffToSelf:
                    bonuses.BuffsToApplyToSelf.Add(effect.EffectData);
                    break;
                case PassiveEffectType.ApplyDebuffToTarget:
                    bonuses.DebuffsToApplyToTarget.Add(effect.EffectData);
                    break;
            }
        }
    }
}