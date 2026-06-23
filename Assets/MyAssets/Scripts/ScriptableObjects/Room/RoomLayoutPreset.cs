using UnityEngine;

[CreateAssetMenu(fileName = "Layout_", menuName = "SlotGame/Room/RoomLayoutPreset")]
public class RoomLayoutPreset : ScriptableObject
{
    [Header("Slot Machine")]
    public bool SlotMachineVisible = true;
    public Vector2 SlotMachinePosition;

    [Header("Enemy Container")]
    public bool EnemyContainerVisible = true;
    public Vector3 EnemyContainerPosition = Vector3.zero;

    [Header("Shop Panel")]
    public bool ShopPanelVisible = false;

    [Header("Player")]
    public Vector3 PlayerPosition = new Vector3(-3f, 0f, 0f);

    [Header("Background")]
    public Sprite BackgroundSprite;
}