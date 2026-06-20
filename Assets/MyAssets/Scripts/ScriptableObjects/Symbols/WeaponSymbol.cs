using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_", menuName = "SlotGame/Symbols/Weapon")]
public class WeaponSymbol : ScriptableObject
{
    public string DisplayName;
    public int BaseDamage;
    public Sprite Icon;
}