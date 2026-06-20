using System;
using UnityEngine;

[Serializable]
public class WeightedEntry<T>
{
    public T Symbol;
    [Min(0)] public float Weight = 1f;
}