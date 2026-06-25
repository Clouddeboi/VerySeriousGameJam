using UnityEngine;

[CreateAssetMenu(fileName = "MapSection_", menuName = "SlotGame/Map/MapSectionConfig")]
public class MapSectionConfigSO : ScriptableObject
{
    public string SectionName;

    [Header("Layout")]
    [Range(1, 10)] public int LayerCount = 4;
    [Range(1, 5)] public int MinNodesPerLayer = 2;
    [Range(1, 5)] public int MaxNodesPerLayer = 3;

    [Header("Room Pools — rooms this section can contain")]
    public RoomDataSO[] CombatRooms;
    public RoomDataSO[] EliteRooms;
    public RoomDataSO[] ShopRooms;

    [Header("Node Type Weights (Boss excluded — handled separately)")]
    public float CombatWeight = 70f;
    public float EliteWeight = 15f;
    public float ShopWeight = 15f;
}