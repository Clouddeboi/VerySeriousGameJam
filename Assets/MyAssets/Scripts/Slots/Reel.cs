using System.Collections.Generic;
using UnityEngine;

public class Reel<T>
{
    private List<WeightedEntry<T>> entries;
    public bool Locked = false;
    public T LastResult;

    public Reel(WeightedEntry<T>[] entryArray)
    {
        entries = new List<WeightedEntry<T>>(entryArray);
    }

    public T Spin()
    {
        if (Locked) return LastResult;

        float total = 0f;
        foreach (var e in entries) total += e.Weight;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var e in entries)
        {
            cumulative += e.Weight;
            if (roll <= cumulative)
            {
                LastResult = e.Symbol;
                return LastResult;
            }
        }

        LastResult = entries[entries.Count - 1].Symbol;
        return LastResult;
    }

    // New: lets RewardSystem's WeightAdjustment actually do something
    public void AdjustWeight(T symbol, float delta)
    {
        foreach (var e in entries)
        {
            if (EqualityComparer<T>.Default.Equals(e.Symbol, symbol))
            {
                e.Weight = Mathf.Max(0f, e.Weight + delta);
                Debug.Log($"[Reel] Adjusted weight of {symbol} by {delta} -> {e.Weight}");
                return;
            }
        }
    }
}