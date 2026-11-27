using System.Collections.Generic;
using UnityEngine;

public class EnemyAIExecutor : MonoBehaviour
{
    public BuffDebuffSystem BuffDebuff;
    public DamageSystem DamageSystem;
    public FloatingTextSpawner TextSpawner;
    public PartyPassiveHandler PassiveHandler;

    public void ExecuteAction(EnemyAI actor, EnemyActionType action,
        List<EnemyAI> allEnemies)
    {
        switch (action)
        {
            case EnemyActionType.Attack:
                ExecuteAttack(actor, allEnemies);
                break;
            case EnemyActionType.Miss:
                Debug.Log($"[EnemyAI] {actor.Data.EnemyName} missed!");
                TextSpawner?.SpawnEffectAppliedText("Miss!", false,
                    actor.transform.position + Vector3.up);
                break;
            case EnemyActionType.Heal:
                ExecuteHeal(actor, allEnemies);
                break;
            case EnemyActionType.Support:
                ExecuteSupport(actor, allEnemies);
                break;
        }
    }

    private void ExecuteAttack(EnemyAI actor, List<EnemyAI> allEnemies)
    {
        actor.PlayAttackAnimationOnly();
        AudioManager.Instance.PlayEnemyAttack();

        var hostileEnemyTarget = FindHostileTarget(actor, allEnemies);
        if (hostileEnemyTarget != null)
        {
            Debug.Log($"[EnemyAI] {actor.Data.EnemyName} attacks hostile {hostileEnemyTarget.Data.EnemyName}");
            DamageSystem.ResolveDamage(actor.Health, hostileEnemyTarget.Health,
                actor.Data.AttackDamage, isEnemyAttack: false,
                actor.Data.AttackElement, hostileEnemyTarget.Data);
            return;
        }

        var partyTarget = SelectPartyTarget(actor.Data.TargetStrategy);
        if (partyTarget == null)
        {
            Debug.Log($"[EnemyAI] {actor.Data.EnemyName} found no valid party target.");
            return;
        }

        Debug.Log($"[EnemyAI] {actor.Data.EnemyName} attacks {partyTarget.Data.CharacterName} for {actor.Data.AttackDamage}");

        bool immune = false;
        foreach (var type in actor.Data.Types)
        {
            if (PassiveHandler.IsImmuneToEnemyType(partyTarget, type))
            {
                Debug.Log($"[Passive] {partyTarget.Data.CharacterName} is immune to {type.TypeName}!");
                TextSpawner?.SpawnEffectAppliedText("Immune!", true,
                    partyTarget.transform.position + Vector3.up * 1.5f);
                immune = true;
                break;
            }
        }

        if (!immune)
        {
            DamageSystem.ResolveDamage(actor.Health, partyTarget.Health,
                actor.Data.AttackDamage, isEnemyAttack: true,
                actor.Data.AttackElement, targetCharacter: partyTarget.Data);

            PassiveHandler.HandlePlayerAttacked(partyTarget, actor,
                new List<EnemyAI>(allEnemies));
        }

        if (actor.Data.AttackBehavior != null)
        {
            foreach (var debuffConfig in actor.Data.AttackBehavior.DebuffsOnHit)
            {
                if (debuffConfig.Debuff == null) continue;

                bool effectImmune = PassiveHandler.IsImmuneToEffect(
                    partyTarget, debuffConfig.Debuff);

                if (!effectImmune && Random.Range(0f, 1f) <= debuffConfig.Chance)
                {
                    int dur = debuffConfig.DurationOverride > 0
                        ? debuffConfig.DurationOverride : -1;
                    BuffDebuff.ApplyEffect(partyTarget.Health, debuffConfig.Debuff, dur);
                    Debug.Log($"[EnemyAI] {actor.Data.EnemyName} applied " +
                        $"{debuffConfig.Debuff.DisplayName} to {partyTarget.Data.CharacterName}");
                }
            }
        }
    }

    private PartyMember SelectPartyTarget(EnemyTargetStrategy strategy)
    {
        var living = GetLivingPartyMembers();
        if (living.Count == 0) return null;

        switch (strategy)
        {
            case EnemyTargetStrategy.Random:
                return living[Random.Range(0, living.Count)];

            case EnemyTargetStrategy.LowestHP:
                PartyMember lowestHP = null;
                int minHP = int.MaxValue;
                foreach (var m in living)
                    if (m.Health.CurrentHP < minHP)
                    { minHP = m.Health.CurrentHP; lowestHP = m; }
                return lowestHP;

            case EnemyTargetStrategy.HighestHP:
                PartyMember highestHP = null;
                int maxHP = int.MinValue;
                foreach (var m in living)
                    if (m.Health.CurrentHP > maxHP)
                    { maxHP = m.Health.CurrentHP; highestHP = m; }
                return highestHP;

            case EnemyTargetStrategy.LowestMaxHP:
                PartyMember lowestMax = null;
                int minMax = int.MaxValue;
                foreach (var m in living)
                    if (m.Health.MaxHP < minMax)
                    { minMax = m.Health.MaxHP; lowestMax = m; }
                return lowestMax;

            default:
                return living[Random.Range(0, living.Count)];
        }
    }

    private List<PartyMember> GetLivingPartyMembers()
    {
        var result = new List<PartyMember>();
        foreach (var m in PartyManager.Instance.ActiveMembers)
            if (m != null && !m.Health.IsDead) result.Add(m);
        return result;
    }

    private void ExecuteHeal(EnemyAI actor, List<EnemyAI> allEnemies)
    {
        if (actor.Data.HealBehavior?.Type == BehaviorType.HealLowestAlly)
        {
            var ally = FindLowestHPAlly(actor, allEnemies);
            if (ally != null && ally != actor)
            {
                Debug.Log($"[EnemyAI] {actor.Data.EnemyName} heals {ally.Data.EnemyName}");
                ally.Health.Heal(actor.Data.HealAmount);
                return;
            }
        }
        Debug.Log($"[EnemyAI] {actor.Data.EnemyName} heals self");
        actor.Health.Heal(actor.Data.HealAmount);
    }

    private void ExecuteSupport(EnemyAI actor, List<EnemyAI> allEnemies)
    {
        if (actor.Data.SupportBehavior == null) return;

        int dur = actor.Data.SupportBehavior.BuffDurationOverride > 0
            ? actor.Data.SupportBehavior.BuffDurationOverride : -1;

        switch (actor.Data.SupportBehavior.Type)
        {
            case BehaviorType.ApplyBuffToSelf:
                if (actor.Data.SupportBehavior.BuffToApply != null)
                    BuffDebuff.ApplyEffect(actor.Health,
                        actor.Data.SupportBehavior.BuffToApply, dur);
                break;

            case BehaviorType.ApplyBuffToAlly:
                var ally = FindLowestHPAlly(actor, allEnemies) ?? actor;
                if (actor.Data.SupportBehavior.BuffToApply != null)
                    BuffDebuff.ApplyEffect(ally.Health,
                        actor.Data.SupportBehavior.BuffToApply, dur);
                break;

            case BehaviorType.ApplyDebuffToPlayer:
                var partyTarget = SelectPartyTarget(EnemyTargetStrategy.Random);
                if (actor.Data.SupportBehavior.BuffToApply != null && partyTarget != null)
                    BuffDebuff.ApplyEffect(partyTarget.Health,
                        actor.Data.SupportBehavior.BuffToApply, dur);
                break;
        }
    }

    private EnemyAI FindHostileTarget(EnemyAI actor, List<EnemyAI> all)
    {
        if (actor.Data.Faction == null) return null;
        foreach (var other in all)
        {
            if (other == actor || other == null || other.Health.IsDead) continue;
            if (other.Data.Faction != null &&
                actor.Data.Faction.IsHostileToward(other.Data.Faction)) return other;
        }
        return null;
    }

    private EnemyAI FindLowestHPAlly(EnemyAI actor, List<EnemyAI> all)
    {
        EnemyAI lowest = null;
        int lowestHP = int.MaxValue;
        foreach (var other in all)
        {
            if (other == null || other.Health.IsDead) continue;
            if (other.Data.Faction != actor.Data.Faction) continue;
            if (other.Health.CurrentHP < lowestHP)
            {
                lowestHP = other.Health.CurrentHP;
                lowest = other;
            }
        }
        return lowest;
    }
}