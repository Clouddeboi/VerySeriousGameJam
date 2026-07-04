using System.Collections.Generic;
using UnityEngine;

public class EnemyAIExecutor : MonoBehaviour
{
    public BuffDebuffSystem BuffDebuff;
    public DamageSystem DamageSystem;
    public FloatingTextSpawner TextSpawner;
    public Health PlayerHealth;

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
                    actor.transform.position + Vector3.up * 1f);
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

        // Check if there's a hostile enemy faction target first
        var hostileTarget = FindHostileTarget(actor, allEnemies);

        if (hostileTarget != null)
        {
            Debug.Log($"[EnemyAI] {actor.Data.EnemyName} attacks hostile {hostileTarget.Data.EnemyName}");
            DamageSystem.ResolveDamage(actor.Health, hostileTarget.Health,
                actor.Data.AttackDamage, isEnemyAttack: false);
        }
        else
        {
            Debug.Log($"[EnemyAI] {actor.Data.EnemyName} attacks player for {actor.Data.AttackDamage}");
            DamageSystem.ResolveDamage(actor.Health, PlayerHealth,
                actor.Data.AttackDamage, isEnemyAttack: true);
        }

        if (actor.Data.AttackBehavior != null && hostileTarget == null)
        {
            foreach (var debuffConfig in actor.Data.AttackBehavior.DebuffsOnHit)
            {
                if (debuffConfig.Debuff == null) continue;
                float roll = Random.Range(0f, 1f);
                if (roll <= debuffConfig.Chance)
                {
                    int duration = debuffConfig.DurationOverride > 0
                        ? debuffConfig.DurationOverride : -1;
                    BuffDebuff.ApplyEffect(PlayerHealth, debuffConfig.Debuff, duration);
                    Debug.Log($"[EnemyAI] {actor.Data.EnemyName} applied {debuffConfig.Debuff.DisplayName} to player");
                }
            }
        }
    }

    private void ExecuteHeal(EnemyAI actor, List<EnemyAI> allEnemies)
    {
        if (actor.Data.HealBehavior?.Type == BehaviorType.HealLowestAlly)
        {
            var target = FindLowestHPAlly(actor, allEnemies);
            if (target != null && target != actor)
            {
                Debug.Log($"[EnemyAI] {actor.Data.EnemyName} heals ally {target.Data.EnemyName}");
                target.Health.Heal(actor.Data.HealAmount);
                return;
            }
        }

        Debug.Log($"[EnemyAI] {actor.Data.EnemyName} heals self for {actor.Data.HealAmount}");
        actor.Health.Heal(actor.Data.HealAmount);
    }

    private void ExecuteSupport(EnemyAI actor, List<EnemyAI> allEnemies)
    {
        if (actor.Data.SupportBehavior == null) return;

        switch (actor.Data.SupportBehavior.Type)
        {
            case BehaviorType.ApplyBuffToSelf:
                if (actor.Data.SupportBehavior.BuffToApply != null)
                {
                    int dur = actor.Data.SupportBehavior.BuffDurationOverride > 0
                        ? actor.Data.SupportBehavior.BuffDurationOverride : -1;
                    BuffDebuff.ApplyEffect(actor.Health,
                        actor.Data.SupportBehavior.BuffToApply, dur);
                    Debug.Log($"[EnemyAI] {actor.Data.EnemyName} buffed self with {actor.Data.SupportBehavior.BuffToApply.DisplayName}");
                }
                break;

            case BehaviorType.ApplyBuffToAlly:
                if (actor.Data.SupportBehavior.BuffToApply != null)
                {
                    var lowestAlly = FindLowestHPAlly(actor, allEnemies);
                    var buffTarget = lowestAlly != null ? lowestAlly : actor;
                    int dur = actor.Data.SupportBehavior.BuffDurationOverride > 0
                        ? actor.Data.SupportBehavior.BuffDurationOverride : -1;
                    BuffDebuff.ApplyEffect(buffTarget.Health,
                        actor.Data.SupportBehavior.BuffToApply, dur);
                    Debug.Log($"[EnemyAI] {actor.Data.EnemyName} applied {actor.Data.SupportBehavior.BuffToApply.DisplayName} to {buffTarget.Data.EnemyName}");
                }
                break;

            case BehaviorType.ApplyDebuffToPlayer:
                if (actor.Data.SupportBehavior.BuffToApply != null)
                {
                    int dur = actor.Data.SupportBehavior.BuffDurationOverride > 0
                        ? actor.Data.SupportBehavior.BuffDurationOverride : -1;
                    BuffDebuff.ApplyEffect(PlayerHealth,
                        actor.Data.SupportBehavior.BuffToApply, dur);
                    Debug.Log($"[EnemyAI] {actor.Data.EnemyName} debuffed player with {actor.Data.SupportBehavior.BuffToApply.DisplayName}");
                }
                break;
        }
    }

    private EnemyAI FindHostileTarget(EnemyAI actor, List<EnemyAI> allEnemies)
    {
        if (actor.Data.Faction == null) return null;

        foreach (var other in allEnemies)
        {
            if (other == actor || other == null || other.Health.IsDead) continue;
            if (other.Data.Faction == null) continue;
            if (actor.Data.Faction.IsHostileToward(other.Data.Faction))
                return other;
        }
        return null;
    }

    private EnemyAI FindLowestHPAlly(EnemyAI actor, List<EnemyAI> allEnemies)
    {
        EnemyAI lowest = null;
        int lowestHP = int.MaxValue;

        foreach (var other in allEnemies)
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