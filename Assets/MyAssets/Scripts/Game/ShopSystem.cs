using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public ShopItemSO[] ShopPool;
    public CurrencySystem Currency;
    public RewardSystem RewardSystem; // reuse ApplyReward so purchase effects match reward effects exactly
    public ShopUI UI;

    public List<ShopItemSO> GenerateStock(int count = 4)
    {
        var pool = new List<ShopItemSO>(ShopPool);
        var stock = new List<ShopItemSO>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            stock.Add(pool[index]);
            pool.RemoveAt(index);
        }
        return stock;
    }

    public void OpenShop(System.Action onExit)
    {
        Debug.Log($"[Shop] OpenShop called. Time: {Time.time}");
        var stock = GenerateStock();
        UI.Show(stock, this, onExit);
        Debug.Log($"[Shop] UI.Show finished. Time: {Time.time}");
    }

    public bool TryPurchase(ShopItemSO item)
    {
        if (!Currency.SpendGold(item.Price))
        {
            Debug.Log($"[Shop] Not enough gold for {item.Reward.DisplayName} (costs {item.Price}, have {Currency.Gold})");
            return false;
        }

        RewardSystem.ApplyReward(item.Reward);
        Debug.Log($"[Shop] Purchased {item.Reward.DisplayName} for {item.Price} gold.");
        return true;
    }
}