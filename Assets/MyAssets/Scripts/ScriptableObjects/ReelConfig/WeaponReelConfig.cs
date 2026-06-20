using UnityEngine;

[CreateAssetMenu(fileName = "WeaponReelConfig", menuName = "SlotGame/Reels/WeaponReelConfig")]
public class WeaponReelConfig : ScriptableObject
{
    public WeightedEntry<WeaponSymbol>[] Entries;
}