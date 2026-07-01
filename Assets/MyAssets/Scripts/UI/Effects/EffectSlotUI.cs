using UnityEngine;
using TMPro;

public class EffectSlotUI : MonoBehaviour
{
    public SpriteRenderer IconRenderer;
    public TextMeshPro CountText;
    public EffectVisualSettings Settings;

    public void Show(ActiveEffect effect)
    {
        gameObject.SetActive(true);

        IconRenderer.sprite = effect.Data.Icon;
        IconRenderer.color = effect.Data.Icon != null
            ? (effect.Data.Category == EffectCategory.Buff ? Settings.BuffColor : Settings.DebuffColor)
            : Settings.NoIconColor;

        CountText.text = effect.StackCount > 1
            ? $"{effect.RemainingTurns}x{effect.StackCount}"
            : effect.RemainingTurns.ToString();

        CountText.color = Settings.TextColor;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}