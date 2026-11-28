using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackExecutionSystem : MonoBehaviour
{
    public DamageSystem DamageSystem;
    public BuffDebuffSystem BuffDebuff;
    //public Health PlayerHealth;
    public SynergyResolver SynergyResolver;

    [Header("Multi-Hit Pacing")]
    public float DelayBetweenHits = 0.15f;

   public void Execute(Attack attack, Health target, ResolvedSynergies synergies,
        List<EnemyAI> allEnemies, EnemyData targetData = null,
        System.Action onComplete = null)
    {
        StartCoroutine(ExecuteRoutine(attack, target, synergies,
            allEnemies, targetData, onComplete));
    }

    private IEnumerator ExecuteRoutine(Attack attack, Health target,
        ResolvedSynergies synergies, List<EnemyAI> allEnemies,
        EnemyData targetData, System.Action onComplete)
    {
        for (int i = 0; i < attack.HitCount; i++)
        {
            DamageSystem.ResolveSingleHit(attack, target, targetData);

            // Self-damage (Confusion) is paid on every hit
            if (attack.SelfDamageAmount > 0)
                attack.Source?.ApplyDamage(attack.SelfDamageAmount);

            if (target.IsDead) break;

            if (i < attack.HitCount - 1)
                yield return new WaitForSeconds(DelayBetweenHits);
            else
                yield return null;
        }

        if (!target.IsDead && attack.Element?.AppliedEffect != null)
            BuffDebuff.ApplyEffect(target, attack.Element.AppliedEffect);

        SynergyResolver.ApplyPostHitEffects(synergies, attack.Source, target, allEnemies);

        // Self effects go to the attacking party member
        var attacker = attack.Source;
        if (attacker != null && attack.SelfAppliedEffect != null)
            BuffDebuff.ApplyEffect(attacker, attack.SelfAppliedEffect);

        onComplete?.Invoke();
    }
}
