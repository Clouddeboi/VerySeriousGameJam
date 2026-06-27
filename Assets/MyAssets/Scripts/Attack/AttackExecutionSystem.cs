using System;
using System.Collections;
using UnityEngine;

public class AttackExecutionSystem : MonoBehaviour
{
    public DamageSystem DamageSystem;
    public StatusEffectSystem StatusEffects;
    public Health PlayerHealth;
    public FloatingTextSpawner TextSpawner;

    public void Execute(Attack attack, Health target, Action onComplete = null)
    {
        StartCoroutine(ExecuteRoutine(attack, target, onComplete));
    }

    private IEnumerator ExecuteRoutine(Attack attack, Health target, System.Action onComplete)
    {
        for (int i = 0; i < attack.HitCount; i++)
        {
            DamageSystem.ResolveSingleHit(attack, target);
            if (target.IsDead) break;
            yield return null;
        }

        if (!target.IsDead && attack.Element != null && attack.Element.Type != ElementType.None)
        {
            if (attack.Element.Type == ElementType.Water)
            {
                StatusEffects.ExtinguishBurn(target);
            }
            else if (attack.Element.Type == ElementType.Ice && attack.FreezeDurationOverride > 0)
            {
                StatusEffects.ApplyFreezeWithDuration(target, attack.FreezeDurationOverride);
            }
            else
            {
                StatusEffects.ApplyFromElement(attack.Element, target);
            }
        }

        if (attack.HealPlayerAmount > 0)
        {
            PlayerHealth.Heal(attack.HealPlayerAmount);
            if (TextSpawner != null)
                TextSpawner.SpawnHealNumber(attack.HealPlayerAmount, PlayerHealth.transform.position + Vector3.up * 0.5f);
        }

        if (attack.ConfusionDamageToPlayer > 0)
        {
            PlayerHealth.ApplyDamage(attack.ConfusionDamageToPlayer);
            if (TextSpawner != null)
                TextSpawner.SpawnDamageNumber(attack.ConfusionDamageToPlayer, PlayerHealth.transform.position + Vector3.up * 0.5f);
        }

        onComplete?.Invoke();
    }
}