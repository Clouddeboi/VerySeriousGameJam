using System;
using System.Collections;
using UnityEngine;

public class AttackExecutionSystem : MonoBehaviour
{
    public DamageSystem DamageSystem;
    public StatusEffectSystem StatusEffects;

    public void Execute(Attack attack, Health target, Action onComplete = null)
    {
        StartCoroutine(ExecuteRoutine(attack, target, onComplete));
    }

    private IEnumerator ExecuteRoutine(Attack attack, Health target, Action onComplete)
    {
        for (int i = 0; i < attack.HitCount; i++)
        {
            DamageSystem.ResolveSingleHit(attack, target);
            if (target.IsDead) break;
            yield return null;
        }

        //Element status effect applies once per attack, regardless of hit count
        if (!target.IsDead && attack.Element != null && attack.Element.Type != ElementType.None)
        {
            if (attack.Element.Type == ElementType.Ice && attack.FreezeDurationOverride > 0)
                StatusEffects.ApplyFreezeWithDuration(target, attack.FreezeDurationOverride);
            else
                StatusEffects.ApplyFromElement(attack.Element, target);
        }

        onComplete?.Invoke();
    }
}