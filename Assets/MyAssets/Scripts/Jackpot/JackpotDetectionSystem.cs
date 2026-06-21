using UnityEngine;

public class JackpotDetectionSystem : MonoBehaviour
{

    public WeaponSymbol Dagger;
    public WeaponSymbol Sword;
    public WeaponSymbol Hammer;

    public JackpotType Detect(SlotResult result)
    {
        bool hasWeapon = result.Weapon != null;
        bool hasElement = result.Element != null;
        bool hasModifier = result.Modifier != null;

        if (!hasWeapon || !hasElement || !hasModifier)
            return JackpotType.None;

        if (result.Weapon == Dagger && result.Element.Type == ElementType.Lightning && result.Modifier.Effect == ModifierEffect.Rapid)
        {
            Debug.Log("[Jackpot] Lightning Dagger!");
            return JackpotType.LightningDagger;
        }

        if (result.Weapon == Sword && result.Element.Type == ElementType.Fire && result.Modifier.Effect == ModifierEffect.Double)
        {
            Debug.Log("[Jackpot] Fire Slash!");
            return JackpotType.FireSlash;
        }

        if (result.Weapon == Hammer && result.Element.Type == ElementType.Ice && result.Modifier.Effect == ModifierEffect.Crit)
        {
            Debug.Log("[Jackpot] Frozen Blast!");
            return JackpotType.FrozenBlast;
        }

        return JackpotType.None;
    }

    public Attack ApplyJackpotBonus(Attack attack, JackpotType jackpot)
    {
        switch (jackpot)
        {
            case JackpotType.LightningDagger:
                attack.HitCount = 6; //instead of Rapid's normal 3
                Debug.Log($"[Jackpot] Hit count boosted to {attack.HitCount}");
                break;

            case JackpotType.FireSlash:
                //Double normally gives x2 in AttackBuilder. Undo that x2, apply x4 instead.
                attack.BaseDamage = Mathf.RoundToInt(attack.BaseDamage / 2f * 4f);
                Debug.Log($"[Jackpot] Damage boosted to x4 -> {attack.BaseDamage}");
                break;

            case JackpotType.FrozenBlast:
                attack.FreezeDurationOverride = 3; //instead of Ice's normal 1
                Debug.Log("[Jackpot] Freeze duration boosted to 3 turns");
                break;
        }
        return attack;
    }
}