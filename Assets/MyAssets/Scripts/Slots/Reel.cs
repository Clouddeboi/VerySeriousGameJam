using System.Collections.Generic;
using UnityEngine;

public class Reel<T>
{
    // Private copies of the config data: the shared config assets are never modified
    private readonly T[] symbols;
    private readonly float[] baseWeights;
    private readonly float[] weights;
    public bool Locked = false;
    public T LastResult;

    public Reel(WeightedEntry<T>[] entryArray)
    {
        int count = entryArray.Length;
        symbols = new T[count];
        baseWeights = new float[count];
        weights = new float[count];
        for (int i = 0; i < count; i++)
        {
            symbols[i] = entryArray[i].Symbol;
            baseWeights[i] = entryArray[i].Weight;
            weights[i] = entryArray[i].Weight;
        }
    }

    public T Spin()
    {
        if (Locked) return LastResult;

        float total = 0f;
        foreach (var w in weights) total += w;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < symbols.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
            {
                LastResult = symbols[i];
                return LastResult;
            }
        }

        LastResult = symbols[symbols.Length - 1];
        return LastResult;
    }

    //Redistributes delta proportionally
    public void AdjustWeight(T symbol, float delta)
    {
        int targetIndex = -1;
        for (int i = 0; i < symbols.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(symbols[i], symbol))
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1) return;

        int othersCount = symbols.Length - 1;
        if (othersCount <= 0)
        {
            weights[targetIndex] = Mathf.Max(0f, weights[targetIndex] + delta);
            return;
        }

        float deltaPerOther = delta / othersCount;

        weights[targetIndex] = Mathf.Max(0f, weights[targetIndex] + delta);

        for (int i = 0; i < symbols.Length; i++)
        {
            if (i == targetIndex) continue;
            weights[i] = Mathf.Max(0f, weights[i] - deltaPerOther);
        }

        Debug.Log($"[Reel] {symbol} +{delta}, others -{deltaPerOther} each");
    }

    public float[] SnapshotWeights() => (float[])weights.Clone();

    // Ignored when null or when the reel layout changed since the weights were saved
    public void ApplyWeights(float[] saved)
    {
        if (saved == null || saved.Length != weights.Length) return;
        System.Array.Copy(saved, weights, weights.Length);
    }

    public void ResetToOriginalWeights()
    {
        System.Array.Copy(baseWeights, weights, weights.Length);
    }
}