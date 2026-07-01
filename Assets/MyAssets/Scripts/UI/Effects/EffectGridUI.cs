using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectGridUI : MonoBehaviour
{
    public EffectVisualSettings Settings;

    private List<EffectSlotUI> slots = new List<EffectSlotUI>();
    private BuffDebuffSystem buffDebuff;
    private Health target;
    private Transform followTarget;

    public void Initialize(BuffDebuffSystem system, Health health, Transform entityTransform)
    {
        buffDebuff = system;
        target = health;
        followTarget = entityTransform;

        BuildSlots();
    }

    private void BuildSlots()
    {
        foreach (var slot in slots)
            if (slot != null) Destroy(slot.gameObject);
        slots.Clear();

        int maxSlots = Settings.Columns * Settings.Rows;

        float totalWidth = Settings.Columns * Settings.CellWidth + (Settings.Columns - 1) * Settings.CellSpacing;
        float totalHeight = Settings.Rows * Settings.CellHeight + (Settings.Rows - 1) * Settings.CellSpacing;
        float startX = -totalWidth / 2f + Settings.CellWidth / 2f;
        float startY = totalHeight / 2f - Settings.CellHeight / 2f;

        for (int i = 0; i < maxSlots; i++)
        {
            int col = i % Settings.Columns;
            int row = i / Settings.Columns;

            float x = startX + col * (Settings.CellWidth + Settings.CellSpacing);
            float y = startY - row * (Settings.CellHeight + Settings.CellSpacing);

            var slotGO = new GameObject($"EffectSlot_{i}");
            slotGO.transform.SetParent(transform);
            slotGO.transform.localPosition = new Vector3(x, y, 0f);

            var iconGO = new GameObject("Icon");
            iconGO.transform.SetParent(slotGO.transform);
            iconGO.transform.localPosition = Vector3.zero;
            iconGO.transform.localScale = new Vector3(
                Settings.CellWidth * Settings.IconScale,
                Settings.CellHeight * Settings.IconScale,
                1f);

            var sr = iconGO.AddComponent<SpriteRenderer>();
            sr.sortingOrder = Settings.IconSortingOrder;

            // Count text
            var textGO = new GameObject("Count");
            textGO.transform.SetParent(slotGO.transform);
            textGO.transform.localPosition = new Vector3(
                Settings.CellWidth * Settings.TextOffset.x,
                Settings.CellHeight * Settings.TextOffset.y,
                -0.1f);
            textGO.transform.localScale = Vector3.one * Settings.TextScale;

            var tmp = textGO.AddComponent<TextMeshPro>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = Settings.FontStyle;
            tmp.fontSize = Settings.FontSize;
            tmp.color = Settings.TextColor;
            tmp.sortingOrder = Settings.TextSortingOrder;

            if (Settings.Font != null)
                tmp.font = Settings.Font;

            var slot = slotGO.AddComponent<EffectSlotUI>();
            slot.IconRenderer = sr;
            slot.CountText = tmp;
            slot.Settings = Settings;

            slot.Hide();
            slots.Add(slot);
        }
    }

    private void LateUpdate()
    {
        if (followTarget != null)
            transform.position = followTarget.position + Settings.WorldOffset;

        if (buffDebuff != null && target != null)
            Refresh(buffDebuff.GetActiveEffects(target));
    }

    private void Refresh(List<ActiveEffect> effects)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < effects.Count && effects[i].Data.Icon != null)
                slots[i].Show(effects[i]);
            else
                slots[i].Hide();
        }
    }
}