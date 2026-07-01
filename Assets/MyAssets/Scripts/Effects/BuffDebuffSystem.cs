using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffSystem : MonoBehaviour
{
    private Dictionary<Health, List<ActiveEffect>> activeEffects = new Dictionary<Health, List<ActiveEffect>>();

    public void ApplyEffect(Health target, EffectDataSO effect, int durationOverride = -1)
    {
        if (effect == null || target == null) return;

        //Interactions resolve FIRST and IMMEDIATELY, regardless of whether this effect itself persists
        if (effect.Interactions != null)
            foreach (var rule in effect.Interactions)
                ResolveInteraction(target, rule);

        int duration = durationOverride > 0 ? durationOverride : effect.DurationTurns;
        if (duration <= 0)
        {
            Debug.Log($"[BuffDebuff] {effect.DisplayName} resolved instantly on {target.EntityName}.");
            return; //instant/interaction-only effect, e.g. Water, never added to the active list
        }

        if (!activeEffects.ContainsKey(target))
            activeEffects[target] = new List<ActiveEffect>();

        var list = activeEffects[target];
        var existing = list.Find(e => e.Data.EffectId == effect.EffectId);

        if (existing == null)
        {
            list.Add(new ActiveEffect { Data = effect, RemainingTurns = duration, StackCount = 1 });
            Debug.Log($"[BuffDebuff] Applied {effect.DisplayName} to {target.EntityName} ({duration} turns)");
        }
        else
        {
            switch (effect.Stacking)
            {
                case StackBehavior.StackDuration: existing.RemainingTurns += duration; break;
                case StackBehavior.StackCount:
                    existing.StackCount++;
                    existing.RemainingTurns = Mathf.Max(existing.RemainingTurns, duration);
                    break;
                case StackBehavior.RefreshDuration: existing.RemainingTurns = duration; break;
                case StackBehavior.ReplaceExisting:
                    existing.RemainingTurns = duration;
                    existing.StackCount = 1;
                    break;
            }
            Debug.Log($"[BuffDebuff] {effect.DisplayName} on {target.EntityName} -> {existing.RemainingTurns} turns, {existing.StackCount} stacks");
        }
    }

    public void RemoveEffect(Health target, EffectDataSO effect)
    {
        if (activeEffects.ContainsKey(target))
            activeEffects[target].RemoveAll(e => e.Data.EffectId == effect.EffectId);
    }

    private void ResolveInteraction(Health target, EffectInteractionRule rule)
    {
        if (!activeEffects.ContainsKey(target) || rule.TargetEffect == null) return;

        var list = activeEffects[target];
        var match = list.Find(e => e.Data.EffectId == rule.TargetEffect.EffectId);
        if (match == null) return;

        if (rule.InteractionType == EffectInteractionType.RemoveCompletely)
        {
            list.Remove(match);
            Debug.Log($"[BuffDebuff] Interaction removed {match.Data.DisplayName} from {target.EntityName}");
        }
        else
        {
            match.StackCount--;
            if (match.StackCount <= 0) list.Remove(match);
        }
    }

    public void TickEffects(Health target)
    {
        if (!activeEffects.ContainsKey(target)) return;

        var list = activeEffects[target];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var effect = list[i];

            if (effect.Data.TicksDamage)
                target.ApplyDamage(effect.Data.TickDamageAmount * effect.StackCount);

            if (effect.Data.HealPerTurn > 0)
                target.Heal(effect.Data.HealPerTurn * effect.StackCount);

            effect.RemainingTurns--;
            if (effect.RemainingTurns <= 0)
            {
                Debug.Log($"[BuffDebuff] {effect.Data.DisplayName} expired on {target.EntityName}");
                list.RemoveAt(i);
            }
        }
    }

    public bool IsActionPrevented(Health target) =>
        activeEffects.ContainsKey(target) && activeEffects[target].Exists(e => e.Data.PreventsAction);

    public float GetOutgoingDamageMultiplier(Health source)
    {
        if (source == null || !activeEffects.ContainsKey(source)) return 1f;
        float mult = 1f;
        foreach (var e in activeEffects[source]) mult *= e.Data.OutgoingDamageMultiplier;
        return mult;
    }

    public bool IsIncomingDamageBlocked(Health target, bool isEnemyAttack)
    {
        if (!activeEffects.ContainsKey(target)) return false;
        foreach (var e in activeEffects[target])
            if (e.Data.BlocksIncomingDamage && (!e.Data.OnlyBlocksEnemyAttacks || isEnemyAttack))
                return true;
        return false;
    }

    public List<ActiveEffect> GetActiveEffects(Health target) =>
        activeEffects.ContainsKey(target) ? activeEffects[target] : new List<ActiveEffect>();
}