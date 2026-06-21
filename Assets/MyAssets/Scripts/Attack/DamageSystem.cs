using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public StatusEffectSystem StatusEffects;

    public void ResolveSingleHit(Attack attack, Health target)
    {
        target.ApplyDamage(attack.BaseDamage);

        if (attack.Element == null || attack.Element.Type == ElementType.None) return;

        if (attack.Element.Type == ElementType.Ice && attack.FreezeDurationOverride > 0)
            StatusEffects.ApplyFreezeWithDuration(target, attack.FreezeDurationOverride);
        else
            StatusEffects.ApplyFromElement(attack.Element, target);
    }
}