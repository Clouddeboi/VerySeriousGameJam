using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSystem : MonoBehaviour
{
    private Dictionary<Health, List<ActiveStatusEffect>> activeEffects = new Dictionary<Health, List<ActiveStatusEffect>>();

    public void ApplyFromElement(ElementSymbol element, Health target)
    {
        if (element == null) return;

        switch (element.Type)
        {
            case ElementType.Fire:
                AddEffect(target, new ActiveStatusEffect { Type = StatusEffectType.Burn, RemainingTurns = 3, Magnitude = 2 });
                break;
            case ElementType.Lightning:
                AddEffect(target, new ActiveStatusEffect { Type = StatusEffectType.Shock, RemainingTurns = 1, Magnitude = 3 });
                break;
            case ElementType.Ice:
                AddEffect(target, new ActiveStatusEffect { Type = StatusEffectType.Freeze, RemainingTurns = 1, Magnitude = 0 });
                break;
        }
    }

    //Lets jackpot bonuses extend/override effect duration directly (e.g. Frozen Blast -> 3 turns instead of 1)
    public void ApplyFreezeWithDuration(Health target, int turns)
    {
        // Remove any existing freeze first so durations don't stack weirdly
        if (activeEffects.ContainsKey(target))
            activeEffects[target].RemoveAll(e => e.Type == StatusEffectType.Freeze);

        AddEffect(target, new ActiveStatusEffect { Type = StatusEffectType.Freeze, RemainingTurns = turns, Magnitude = 0 });
    }

    public bool IsFrozen(Health target)
    {
        if (!activeEffects.ContainsKey(target)) return false;
        foreach (var e in activeEffects[target])
            if (e.Type == StatusEffectType.Freeze) return true;
        return false;
    }

    private void AddEffect(Health target, ActiveStatusEffect effect)
    {
        if (!activeEffects.ContainsKey(target))
            activeEffects[target] = new List<ActiveStatusEffect>();

        activeEffects[target].Add(effect);
        Debug.Log($"[StatusEffect] Applied {effect.Type} to {target.EntityName} ({effect.RemainingTurns} turns, mag {effect.Magnitude})");
    }

    public void TickEffects(Health target)
    {
        if (!activeEffects.ContainsKey(target)) return;

        var list = activeEffects[target];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var effect = list[i];

            if (effect.Type == StatusEffectType.Burn)
            {
                target.ApplyDamage(effect.Magnitude);
                Debug.Log($"[StatusEffect] {target.EntityName} burns for {effect.Magnitude}");
            }

            effect.RemainingTurns--;
            if (effect.RemainingTurns <= 0)
            {
                Debug.Log($"[StatusEffect] {effect.Type} expired on {target.EntityName}");
                list.RemoveAt(i);
            }
        }
    }

    public int GetShockBonus(Health target)
    {
        if (!activeEffects.ContainsKey(target)) return 0;

        int bonus = 0;
        foreach (var e in activeEffects[target])
            if (e.Type == StatusEffectType.Shock) bonus += e.Magnitude;
        return bonus;
    }

    public List<StatusEffectType> GetActiveEffectTypes(Health target)
    {
        var result = new List<StatusEffectType>();
        if (!activeEffects.ContainsKey(target)) return result;

        foreach (var e in activeEffects[target])
            if (!result.Contains(e.Type)) result.Add(e.Type);

        return result;
    }
}