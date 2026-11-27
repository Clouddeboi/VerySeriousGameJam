using UnityEngine;
using System.Collections.Generic;

public class StatusEffectVisual : MonoBehaviour
{
    public BuffDebuffSystem BuffDebuff;
    public Health Target;
    public SpriteRenderer OverlayRenderer;
    [Range(0f, 1f)] public float OverlayOpacity = 0.5f;

    private BuffDebuffSystem subscribedTo;

    public void Initialize(BuffDebuffSystem buffDebuff, Health target)
    {
        Unsubscribe();
        BuffDebuff = buffDebuff;
        Target = target;
        if (BuffDebuff != null)
        {
            subscribedTo = BuffDebuff;
            subscribedTo.OnEffectsChanged += HandleEffectsChanged;
        }
        Refresh();
    }

    private void OnDestroy() => Unsubscribe();

    private void Unsubscribe()
    {
        if (subscribedTo != null) subscribedTo.OnEffectsChanged -= HandleEffectsChanged;
        subscribedTo = null;
    }

    private void HandleEffectsChanged(Health changed)
    {
        if (changed == Target) Refresh();
    }

    private void Refresh()
    {
        if (BuffDebuff == null || Target == null || OverlayRenderer == null) return;

        var active = BuffDebuff.GetActiveEffects(Target);
        for (int i = 0; i < active.Count; i++)
        {
            if (active[i].Data.Icon != null)
            {
                OverlayRenderer.sprite = active[i].Data.Icon;
                SetAlpha(OverlayOpacity);
                return;
            }
        }
        SetAlpha(0f);
    }

    private void SetAlpha(float a) { var c = OverlayRenderer.color; c.a = a; OverlayRenderer.color = c; }
}