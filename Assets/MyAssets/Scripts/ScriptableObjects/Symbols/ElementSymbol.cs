using UnityEngine;

[CreateAssetMenu(fileName = "Element_", menuName = "SlotGame/Symbols/Element")]
public class ElementSymbol : ScriptableObject
{
    public string DisplayName;
    public ElementType Type;
    public Sprite Icon;
}