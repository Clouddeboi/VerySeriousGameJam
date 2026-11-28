using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffSystem : MonoBehaviour
{
    private Dictionary<Health, List<ActiveEffect>> activeEffects
        = new Dictionary<Health, List<ActiveEffect>>();

    private static readonly List<ActiveEffect> emptyEffects = new List<ActiveEffect>();
    private readonly List<Health> staleTargets = new List<Health>();

    public System.Action<Health, ActiveEffect> OnEffectApplied;
    public System.Action<Health, ActiveEffect> OnEffectTick;
    public System.Action<Health, string> OnInteraction;
    public System.Action<Health> OnEffectsChanged;

    private void Awake()
    {
        activeEffects.Clear();
    }

    public void ApplyEffect(Health target, EffectDataSO data, int durationOverride = -1)
    {
        if (target == null || data == null || target.IsDead) return;

        PruneDestroyedTargets();

        if (data.Interactions != null)
        {
            foreach (var rule in data.Interactions)
                ResolveInteraction(target, rule, data.DisplayName);
        }

        int duration = durationOverride > 0 ? durationOverride : data.DurationTurns;

        if (duration <= 0) return;

        if (!activeEffects.ContainsKey(target))
            activeEffects[target] = new List<ActiveEffect>();

        var list = activeEffects[target];
        var existing = list.Find(e => e.Data.EffectId == data.EffectId);

        if (existing != null)
        {
            switch (data.Stacking)
            {
                case StackBehavior.StackDuration:
                    existing.RemainingTurns += duration;
                    break;
                case StackBehavior.StackCount:
                    existing.StackCount++;
                    existing.RemainingTurns = Mathf.Max(existing.RemainingTurns, duration);
                    break;
                case StackBehavior.RefreshDuration:
                    existing.RemainingTurns = duration;
                    break;
                case StackBehavior.ReplaceExisting:
                    existing.RemainingTurns = duration;
                    existing.StackCount = 1;
                    break;
            }
            Debug.Log($"[BuffDebuff] {data.DisplayName} stacked on {target.EntityName} -> {existing.RemainingTurns} turns, {existing.StackCount} stacks");
            OnEffectApplied?.Invoke(target, existing);
        }
        else
        {
            var effect = new ActiveEffect { Data = data, RemainingTurns = duration, StackCount = 1 };
            list.Add(effect);
            Debug.Log($"[BuffDebuff] Applied {data.DisplayName} to {target.EntityName} ({duration} turns)");
            OnEffectApplied?.Invoke(target, effect);
        }

        OnEffectsChanged?.Invoke(target);
    }

    public void RemoveEffect(Health target, EffectDataSO data)
    {
        if (data == null || !activeEffects.ContainsKey(target)) return;
        int removed = activeEffects[target].RemoveAll(e => e.Data.EffectId == data.EffectId);
        if (removed > 0)
        {
            Debug.Log($"[BuffDebuff] Removed {data.DisplayName} from {target.EntityName}");
            OnEffectsChanged?.Invoke(target);
        }
    }

    public void TickEffects(Health target)
    {
        if (target == null || !activeEffects.ContainsKey(target)) return;

        var list = activeEffects[target];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var effect = list[i];

            if (effect.Data.TicksDamage && effect.Data.TickDamageAmount > 0)
            {
                target.ApplyDamage(effect.Data.TickDamageAmount * effect.StackCount);
                OnEffectTick?.Invoke(target, effect);
            }

            if (effect.Data.HealPerTurn > 0)
            {
                target.Heal(effect.Data.HealPerTurn * effect.StackCount);
                OnEffectTick?.Invoke(target, effect);
            }

            effect.RemainingTurns--;
            if (effect.RemainingTurns <= 0)
            {
                Debug.Log($"[BuffDebuff] {effect.Data.DisplayName} expired on {target.EntityName}");
                list.RemoveAt(i);
            }
        }

        if (list.Count == 0)
            activeEffects.Remove(target);

        OnEffectsChanged?.Invoke(target);
    }

    private void ResolveInteraction(Health target, EffectInteractionRule rule, string sourceName)
    {
        if (rule.TargetEffect == null || !activeEffects.ContainsKey(target)) return;

        var list = activeEffects[target];
        var match = list.Find(e => e.Data.EffectId == rule.TargetEffect.EffectId);
        if (match == null) return;

        string interactionText = $"{rule.TargetEffect.DisplayName} removed!";

        switch (rule.InteractionType)
        {
            case EffectInteractionType.RemoveCompletely:
                list.Remove(match);
                Debug.Log($"[BuffDebuff] {sourceName} removed {match.Data.DisplayName} from {target.EntityName}");
                break;
            case EffectInteractionType.ReduceOneStack:
                match.StackCount--;
                if (match.StackCount <= 0) list.Remove(match);
                Debug.Log($"[BuffDebuff] {sourceName} reduced {match.Data.DisplayName} stack on {target.EntityName}");
                interactionText = $"{rule.TargetEffect.DisplayName} weakened!";
                break;
        }

        OnInteraction?.Invoke(target, interactionText);
        OnEffectsChanged?.Invoke(target);
    }

    public bool IsActionPrevented(Health target)
    {
        if (!activeEffects.ContainsKey(target)) return false;
        return activeEffects[target].Exists(e => e.Data.PreventsAction);
    }

    public bool IsIncomingDamageBlocked(Health target, bool isEnemyAttack)
    {
        if (!activeEffects.ContainsKey(target)) return false;
        return activeEffects[target].Exists(e =>
            e.Data.BlocksIncomingDamage &&
            (!e.Data.OnlyBlocksEnemyAttacks || isEnemyAttack));
    }

    public float GetOutgoingDamageMultiplier(Health source)
    {
        if (source == null || !activeEffects.ContainsKey(source)) return 1f;
        float mult = 1f;
        foreach (var e in activeEffects[source])
            mult *= e.Data.OutgoingDamageMultiplier;
        return mult;
    }

    public float GetIncomingDamageMultiplier(Health target)
    {
        if (target == null || !activeEffects.ContainsKey(target)) return 1f;
        float mult = 1f;
        foreach (var e in activeEffects[target])
            mult *= e.Data.IncomingDamageMultiplier;
        return mult;
    }

    public float GetReflectPercent(Health target)
    {
        if (target == null || !activeEffects.ContainsKey(target)) return 0f;
        float total = 0f;
        foreach (var e in activeEffects[target])
            total += e.Data.ReflectPercent;
        return Mathf.Clamp01(total);
    }

    // Returns the live list (no copy); callers must not modify it or hold it across effect changes.
    public IReadOnlyList<ActiveEffect> GetActiveEffects(Health target)
    {
        if (target != null && activeEffects.TryGetValue(target, out var list)) return list;
        return emptyEffects;
    }

    private void PruneDestroyedTargets()
    {
        staleTargets.Clear();
        foreach (var key in activeEffects.Keys)
            if (key == null) staleTargets.Add(key);
        foreach (var key in staleTargets)
            activeEffects.Remove(key);
    }

    public bool HasEffect(Health target, EffectDataSO data)
    {
        if (!activeEffects.ContainsKey(target)) return false;
        return activeEffects[target].Exists(e => e.Data.EffectId == data.EffectId);
    }
}