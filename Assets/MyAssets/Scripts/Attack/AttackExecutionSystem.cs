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

    [Header("Multi-Hit Pacing")]
    public float DelayBetweenHits = 0.15f;

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
            int hpBefore = target.CurrentHP;
            DamageSystem.ResolveSingleHit(attack, target);
            int damageDealt = hpBefore - target.CurrentHP;

            //Spawn damage number per hit, offset slightly each time so stacked numbers don't overlap
            if (damageDealt > 0 && TextSpawner != null)
            {
                Vector3 offset = Vector3.up * (0.5f + i * 0.3f); //Each hit floats a little higher
                TextSpawner.SpawnDamageNumber(damageDealt, target.transform.position + offset);
            }

            if (target.IsDead) break;

            if (i < attack.HitCount - 1)
                yield return new WaitForSeconds(DelayBetweenHits);
            else
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

        if (synergies.HealSelfAmount > 0)
            TextSpawner?.SpawnHealNumber(synergies.HealSelfAmount,
                PlayerHealth.transform.position + Vector3.up * 0.5f);

        onComplete?.Invoke();
    }
}