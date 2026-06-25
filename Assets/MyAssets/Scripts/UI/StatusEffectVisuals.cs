using UnityEngine;
using System.Collections.Generic;

public class StatusEffectVisual : MonoBehaviour
{
    public StatusEffectSystem StatusEffects;
    public Health Target;
    public SpriteRenderer OverlayRenderer;
    public StatusIconData[] IconData;

    [Range(0f, 1f)] public float OverlayOpacity = 0.5f;

    private Dictionary<StatusEffectType, Sprite> iconLookup;
    private bool initialized = false;

    public void Initialize()
    {
        iconLookup = new Dictionary<StatusEffectType, Sprite>();
        foreach (var data in IconData)
            iconLookup[data.Type] = data.Icon;

        SetOverlayAlpha(OverlayRenderer, 0f);
        initialized = true;
    }

    private void Update()
    {
        if (!initialized || StatusEffects == null || Target == null) return;

        var active = StatusEffects.GetActiveEffectTypes(Target);

        if (active.Count == 0)
        {
            SetOverlayAlpha(OverlayRenderer, 0f);
            return;
        }

        var type = active[0];
        if (iconLookup.TryGetValue(type, out Sprite icon))
        {
            OverlayRenderer.sprite = icon;
            SetOverlayAlpha(OverlayRenderer, OverlayOpacity);
        }
    }

    private void SetOverlayAlpha(SpriteRenderer renderer, float alpha)
    {
        Color c = renderer.color;
        c.a = alpha;
        renderer.color = c;
    }
}