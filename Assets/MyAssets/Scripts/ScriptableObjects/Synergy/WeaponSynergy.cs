using UnityEngine;

[CreateAssetMenu(fileName = "Synergy_", menuName = "SlotGame/Synergy/WeaponSynergy")]
public class WeaponSynergy : ScriptableObject
{
    public string SynergyName;

    [Header("Conditions — leave null to match any")]
    public WeaponSymbol RequiredWeapon;
    public ElementSymbol RequiredElement;
    public ModifierSymbol RequiredModifier;

    [Header("Effects Granted When Triggered")]
    public SynergyEffect[] Effects;

    public bool Matches(SlotResult result)
    {
        if (RequiredWeapon != null && result.Weapon != RequiredWeapon) return false;
        if (RequiredElement != null && result.Element != RequiredElement) return false;
        if (RequiredModifier != null && result.Modifier != RequiredModifier) return false;
        return true;
    }
}