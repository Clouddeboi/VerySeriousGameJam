using UnityEngine;

[CreateAssetMenu(fileName = "Modifier_", menuName = "SlotGame/Symbols/Modifier")]
public class ModifierSymbol : ScriptableObject
{
    public string DisplayName;
    public ModifierEffect Effect;
    public Sprite Icon;
}