using UnityEngine;

public class AttackBuilder : MonoBehaviour
{
    public Attack Build(SlotResult result)
    {
        int baseDamage = result.Weapon ? result.Weapon.BaseDamage : 1;
        int hitCount = 1;
        bool isCrit = false;

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
        }

        var attack = new Attack
        {
            BaseDamage = baseDamage,
            HitCount = hitCount,
            Element = result.Element,
            IsCrit = isCrit
        };

        Debug.Log($"[AttackBuilder] Built: {attack}");
        return attack;
    }
}