using UnityEngine;

public class EffectTextCoordinator : MonoBehaviour
{
    public BuffDebuffSystem BuffDebuff;
    public FloatingTextSpawner TextSpawner;

    [Header("Vertical Offsets")]
    public float AppliedTextOffset = 1.5f;
    public float TickTextOffset = 1.2f;
    public float InteractionTextOffset = 1.8f;

    private void Start()
    {
        BuffDebuff.OnEffectApplied += HandleEffectApplied;
        BuffDebuff.OnEffectTick += HandleEffectTick;
        BuffDebuff.OnInteraction += HandleInteraction;
    }

    private void HandleEffectApplied(Health target, ActiveEffect effect)
    {
        if (target == null) return;

        bool isBuff = effect.Data.Category == EffectCategory.Buff;

        string text = effect.StackCount > 1
            ? $"{effect.Data.DisplayName} x{effect.StackCount}"
            : effect.Data.DisplayName;

        TextSpawner.SpawnEffectAppliedText(text, isBuff,
            target.transform.position + Vector3.up * AppliedTextOffset);
    }

    private void HandleEffectTick(Health target, ActiveEffect effect)
    {
        if (target == null) return;
        
        string label = effect.Data.TicksDamage
            ? effect.Data.DisplayName
            : $"+{effect.Data.DisplayName}";

        TextSpawner.SpawnEffectTickText(label,
            target.transform.position + Vector3.up * TickTextOffset);
    }

    private void HandleInteraction(Health target, string message)
    {
        if (target == null) return;

        TextSpawner.SpawnInteractionText(message,
            target.transform.position + Vector3.up * InteractionTextOffset);
    }

    private void OnDestroy()
    {
        if (BuffDebuff == null) return;
        BuffDebuff.OnEffectApplied -= HandleEffectApplied;
        BuffDebuff.OnEffectTick -= HandleEffectTick;
        BuffDebuff.OnInteraction -= HandleInteraction;
    }
}