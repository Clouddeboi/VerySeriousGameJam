using System;
using System.Collections;
using UnityEngine;

public class AttackExecutionSystem : MonoBehaviour
{
    public DamageSystem DamageSystem;
    public BuffDebuffSystem BuffDebuff;
    public Health PlayerHealth;
    public FloatingTextSpawner TextSpawner;

    public void Execute(Attack attack, Health target, System.Action onComplete = null) =>
        StartCoroutine(ExecuteRoutine(attack, target, onComplete));

    private IEnumerator ExecuteRoutine(Attack attack, Health target, System.Action onComplete)
    {
        for (int i = 0; i < attack.HitCount; i++)
        {
            DamageSystem.ResolveSingleHit(attack, target);
            if (target.IsDead) break;
            yield return null;
        }

        if (!target.IsDead && attack.Element != null && attack.Element.AppliedEffect != null)
        {
            int durationOverride = attack.Element.Type == ElementType.Ice ? attack.FreezeDurationOverride : -1;
            BuffDebuff.ApplyEffect(target, attack.Element.AppliedEffect, durationOverride);
        }

        if (attack.AppliedPlayerBuff != null)
            BuffDebuff.ApplyEffect(PlayerHealth, attack.AppliedPlayerBuff);

        if (attack.ConfusionDamageToPlayer > 0)
        {
            PlayerHealth.ApplyDamage(attack.ConfusionDamageToPlayer);
            TextSpawner?.SpawnDamageNumber(attack.ConfusionDamageToPlayer, PlayerHealth.transform.position + Vector3.up * 0.5f);
        }

        onComplete?.Invoke();
    }
}