using UnityEngine;

public class AttackBuilder : MonoBehaviour
{
    public WeaponSymbol TridentWeapon;
    public EffectDataSO RegenerationEffect;

    public Attack Build(SlotResult result)
    {
        int baseDamage = result.Weapon ? result.Weapon.BaseDamage : 1;
        int hitCount = 1;
        bool isCrit = false, isOneShot = false;
        EffectDataSO appliedBuff = null;
        int confusionDamage = 0;

        ModifierEffect effect = result.Modifier ? result.Modifier.Effect : ModifierEffect.None;
        switch (effect)
        {
            case ModifierEffect.Double: baseDamage *= 2; break;
            case ModifierEffect.Rapid: hitCount = 3; baseDamage = Mathf.RoundToInt(baseDamage * 0.5f); break;
            case ModifierEffect.Crit: isCrit = true; baseDamage = Mathf.RoundToInt(baseDamage * 1.5f); break;
            case ModifierEffect.OneShot: isOneShot = true; baseDamage = 999; break;
            case ModifierEffect.Heal: appliedBuff = RegenerationEffect; break;
            case ModifierEffect.Confusion: confusionDamage = Mathf.RoundToInt(baseDamage * 0.25f); break;
        }

        if (result.Weapon == TridentWeapon && result.Element != null && result.Element.Type == ElementType.Water)
            baseDamage += 15;

        return new Attack
        {
            BaseDamage = baseDamage,
            HitCount = hitCount,
            Element = result.Element,
            Weapon = result.Weapon,
            IsCrit = isCrit,
            IsOneShot = isOneShot,
            ConfusionDamageToPlayer = confusionDamage,
            AppliedPlayerBuff = appliedBuff
        };
    }
}