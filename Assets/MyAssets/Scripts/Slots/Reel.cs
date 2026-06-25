using System.Collections.Generic;
using UnityEngine;

public class Reel<T>
{
    private List<WeightedEntry<T>> entries;
    private List<float> originalWeights;
    public bool Locked = false;
    public T LastResult;

    public Reel(WeightedEntry<T>[] entryArray)
    {
        entries = new List<WeightedEntry<T>>(entryArray);
        originalWeights = new List<float>();
        foreach (var e in entries) originalWeights.Add(e.Weight);
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

    //Redistributes delta proportionally
    public void AdjustWeight(T symbol, float delta)
    {
        int targetIndex = -1;
        for (int i = 0; i < entries.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(entries[i].Symbol, symbol))
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1) return;

        int othersCount = entries.Count - 1;
        if (othersCount <= 0)
        {
            entries[targetIndex].Weight = Mathf.Max(0f, entries[targetIndex].Weight + delta);
            return;
        }

        float deltaPerOther = delta / othersCount;

        entries[targetIndex].Weight = Mathf.Max(0f, entries[targetIndex].Weight + delta);

        for (int i = 0; i < entries.Count; i++)
        {
            if (i == targetIndex) continue;
            entries[i].Weight = Mathf.Max(0f, entries[i].Weight - deltaPerOther);
        }

        Debug.Log($"[Reel] {symbol} +{delta}, others -{deltaPerOther} each");
    }

    public void ResetToOriginalWeights()
    {
        for (int i = 0; i < entries.Count; i++)
            entries[i].Weight = originalWeights[i];
    }
}