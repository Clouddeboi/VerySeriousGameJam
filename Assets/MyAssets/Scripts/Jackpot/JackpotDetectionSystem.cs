using UnityEngine;

public class JackpotDetectionSystem : MonoBehaviour
{
    [Header("Weapon References")]
    public WeaponSymbol Dagger;
    public WeaponSymbol Sword;
    public WeaponSymbol Hammer;

    [Header("Modifier References")]
    public ModifierSymbol RapidModifier;
    public ModifierSymbol DoubleModifier;
    public ModifierSymbol CritModifier;

    public JackpotType Detect(SlotResult result)
    {
        if (result.Weapon == Dagger && result.Element?.Type == ElementType.Lightning && result.Modifier == RapidModifier)
        {
            Debug.Log("[Jackpot] Lightning Dagger!");
            return JackpotType.LightningDagger;
        }
        if (result.Weapon == Sword && result.Element?.Type == ElementType.Fire && result.Modifier == DoubleModifier)
        {
            Debug.Log("[Jackpot] Fire Slash!");
            return JackpotType.FireSlash;
        }
        if (result.Weapon == Hammer && result.Element?.Type == ElementType.Ice && result.Modifier == CritModifier)
        {
            Debug.Log("[Jackpot] Frozen Blast!");
            return JackpotType.FrozenBlast;
        }
        return JackpotType.None;
    }

    public void TriggerVisuals(JackpotType jackpot)
    {
        if (jackpot == JackpotType.None) return;
        Debug.Log($"[Jackpot] Visual: {jackpot}");
        //AudioManager.Instance.PlayJackpot();
        //TextSpawner.SpawnSlotResultText("JACKPOT!", ...)
    }
}