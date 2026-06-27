using UnityEngine;
using System;

public class CurrencySystem : MonoBehaviour
{
    public int Gold { get; private set; }
    public event Action<int> OnGoldChanged;

    public void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"[Currency] ({GetInstanceID()}) +{amount} gold -> {Gold} total");
        OnGoldChanged?.Invoke(Gold);
    }

    public bool SpendGold(int amount)
    {
        if (Gold < amount) return false;
        Gold -= amount;
        Debug.Log($"[Currency] ({GetInstanceID()}) -{amount} gold -> {Gold} total");
        OnGoldChanged?.Invoke(Gold);
        return true;
    }

    public void UpdateText()
    {
        OnGoldChanged?.Invoke(Gold);
    }
}