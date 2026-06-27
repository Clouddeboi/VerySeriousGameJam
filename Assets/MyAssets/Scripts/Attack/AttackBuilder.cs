using UnityEngine;

public class AttackBuilder : MonoBehaviour
{
    public Attack Build(SlotResult result)
    {
        int baseDamage = result.Weapon ? result.Weapon.BaseDamage : 1;
        int hitCount = 1;
        bool isCrit = false;
        bool isOneShot = false;

        ModifierEffect effect = result.Modifier ? result.Modifier.Effect : ModifierEffect.None;

        switch (effect)
        {
            case ModifierEffect.Double:
                baseDamage *= 2;
                break;
            case ModifierEffect.Rapid:
                hitCount = 3;
                baseDamage = Mathf.RoundToInt(baseDamage * 0.5f);
                break;
            case ModifierEffect.Crit:
                isCrit = true;
                baseDamage = Mathf.RoundToInt(baseDamage * 1.5f);
                break;
            case ModifierEffect.OneShot:
                isOneShot = true;
                baseDamage = 999;
                break;
        }

        if (result.Weapon != null && result.Weapon.name == "Weapon_Trident" &&
            result.Element != null && result.Element.Type == ElementType.Water)
        {
            baseDamage += 15;
            Debug.Log("[AttackBuilder] Trident + Water synergy: +15 damage");
        }

        int healAmount = 0;
        int confusionDamage = 0;

        if (effect == ModifierEffect.Heal)
            healAmount = Mathf.RoundToInt(baseDamage * 0.5f);
        else if (effect == ModifierEffect.Confusion)
            confusionDamage = Mathf.RoundToInt(baseDamage * 0.25f);

        var attack = new Attack
        {
            BaseDamage = baseDamage,
            HitCount = hitCount,
            Element = result.Element,
            Weapon = result.Weapon,
            IsCrit = isCrit,
            IsOneShot = isOneShot,
            HealPlayerAmount = healAmount,
            ConfusionDamageToPlayer = confusionDamage
        };

        Debug.Log($"[AttackBuilder] Built: {attack}");
        return attack;
    }
}