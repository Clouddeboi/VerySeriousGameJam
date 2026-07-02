using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackExecutionSystem : MonoBehaviour
{
    public DamageSystem DamageSystem;
    public BuffDebuffSystem BuffDebuff;
    public Health PlayerHealth;
    public FloatingTextSpawner TextSpawner;
    public SynergyResolver SynergyResolver;

    public void Execute(Attack attack, Health target, ResolvedSynergies synergies,
        List<EnemyAI> allEnemies, Action onComplete = null)
    {
        StartCoroutine(ExecuteRoutine(attack, target, synergies, allEnemies, onComplete));
    }

    private IEnumerator ExecuteRoutine(Attack attack, Health target, ResolvedSynergies synergies,
        List<EnemyAI> allEnemies, Action onComplete)
    {
        for (int i = 0; i < attack.HitCount; i++)
        {
            DamageSystem.ResolveSingleHit(attack, target);
            if (target.IsDead) break;
            yield return null;
        }

        if (!target.IsDead && attack.Element?.AppliedEffect != null)
            BuffDebuff.ApplyEffect(target, attack.Element.AppliedEffect);

        SynergyResolver.ApplyPostHitEffects(synergies, target, allEnemies);

        if (attack.SelfDamageAmount > 0)
        {
            PlayerHealth.ApplyDamage(attack.SelfDamageAmount);
            TextSpawner?.SpawnDamageNumber(attack.SelfDamageAmount,
                PlayerHealth.transform.position + Vector3.up * 0.5f);
        }

        if (attack.SelfAppliedEffect != null)
            BuffDebuff.ApplyEffect(PlayerHealth, attack.SelfAppliedEffect);

        onComplete?.Invoke();
    }
}