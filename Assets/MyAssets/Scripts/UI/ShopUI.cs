using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public GameObject PanelRoot;
    public ShopItemButton[] ItemButtons;
    public TMP_Text GoldText;
    public UnityEngine.UI.Button ExitButton;
    public TransitionController Transition;

    private ShopSystem shop;
    private CurrencySystem currency;
    private System.Action pendingExit;

    public void Show(List<ShopItemSO> stock, ShopSystem shopSystem, System.Action onExit)
    {
        shop = shopSystem;
        currency = shopSystem.Currency;
        pendingExit = onExit;

        PanelRoot.SetActive(true);
        RefreshGoldText();

        for (int i = 0; i < ItemButtons.Length; i++)
        {
            if (i < stock.Count)
            {
                ItemButtons[i].gameObject.SetActive(true);
                ItemButtons[i].Setup(stock[i], HandlePurchaseClicked);
            }
            else
            {
                ItemButtons[i].gameObject.SetActive(false);
            }
        }

        ExitButton.onClick.RemoveAllListeners();
        ExitButton.onClick.AddListener(HandleExitClicked);
    }

    private void HandleExitClicked()
    {
        ExitButton.interactable = false;

        Transition.PlayTransitionOneWay(() =>
        {
            PanelRoot.SetActive(false);
            ExitButton.interactable = true;
            pendingExit?.Invoke();
        });
    }

    private void HandlePurchaseClicked(ShopItemSO item)
    {
        bool success = shop.TryPurchase(item);
        if (success)
        {
            RefreshGoldText();
            foreach (var btn in ItemButtons)
                if (btn.CurrentItem == item) btn.SetPurchased();
        }
    }

    private void RefreshGoldText()
    {
        //GoldText.text = $"Gold: {currency.Gold}";
    }
}