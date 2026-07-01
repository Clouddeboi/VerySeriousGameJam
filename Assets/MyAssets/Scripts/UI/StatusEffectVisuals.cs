using UnityEngine;
using System.Collections.Generic;

public class StatusEffectVisual : MonoBehaviour
{
    public BuffDebuffSystem BuffDebuff;
    public Health Target;
    public SpriteRenderer OverlayRenderer;
    [Range(0f, 1f)] public float OverlayOpacity = 0.5f;

    private void Update()
    {
        if (BuffDebuff == null || Target == null) return;

        var active = BuffDebuff.GetActiveEffects(Target);
        foreach (var effect in active)
        {
            if (effect.Data.Icon != null)
            {
                OverlayRenderer.sprite = effect.Data.Icon;
                SetAlpha(OverlayOpacity);
                return;
            }
        }
        SetAlpha(0f);
    }

    private void SetAlpha(float a) { var c = OverlayRenderer.color; c.a = a; OverlayRenderer.color = c; }
}