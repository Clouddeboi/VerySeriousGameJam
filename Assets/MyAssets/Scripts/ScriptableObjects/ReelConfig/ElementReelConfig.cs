using UnityEngine;

[CreateAssetMenu(fileName = "ElementReelConfig", menuName = "SlotGame/Reels/ElementReelConfig")]
public class ElementReelConfig : ScriptableObject
{
    public WeightedEntry<ElementSymbol>[] Entries;
}