using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
    public int Gold { get; private set; }

    public void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"[Currency] +{amount} gold -> {Gold} total");
    }

    public bool SpendGold(int amount)
    {
        if (Gold < amount) return false;
        Gold -= amount;
        Debug.Log($"[Currency] -{amount} gold -> {Gold} total");
        return true;
    }
}