using UnityEngine;
using TMPro;

public class GoldDisplayUI : MonoBehaviour
{
    public CurrencySystem Currency;
    public TMP_Text GoldText;

    private void Start()
    {
        Currency.OnGoldChanged += UpdateText;
        Currency.UpdateText();
    }

    private void UpdateText(int amount)
    {
        GoldText.text = amount.ToString();
    }

    private void OnDestroy()
    {
        if (Currency != null)
            Currency.OnGoldChanged -= UpdateText;
    }
}