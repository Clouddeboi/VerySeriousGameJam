using UnityEngine;

[CreateAssetMenu(fileName = "StatusIcon_", menuName = "SlotGame/Status/StatusIconData")]
public class StatusIconData : ScriptableObject
{
    public StatusEffectType Type;
    public Sprite Icon;
}