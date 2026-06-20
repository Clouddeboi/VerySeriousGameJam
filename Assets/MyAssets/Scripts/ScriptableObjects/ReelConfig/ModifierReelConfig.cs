using UnityEngine;

[CreateAssetMenu(fileName = "ModifierReelConfig", menuName = "SlotGame/Reels/ModifierReelConfig")]
public class ModifierReelConfig : ScriptableObject
{
    public WeightedEntry<ModifierSymbol>[] Entries;
}