using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
    public TMP_Text NameText;
    public TMP_Text PriceText;
    public Button Button;

    public ShopItemSO CurrentItem { get; private set; }
    private Action<ShopItemSO> onClick;

    public void Setup(ShopItemSO item, Action<ShopItemSO> callback)
    {
        CurrentItem = item;
        onClick = callback;
        NameText.text = item.Reward.DisplayName;
        PriceText.text = $"{item.Price}g";

        Button.interactable = true;
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => onClick?.Invoke(CurrentItem));
    }

    public void SetPurchased()
    {
        Button.interactable = false;
        PriceText.text = "Sold";
    }
}