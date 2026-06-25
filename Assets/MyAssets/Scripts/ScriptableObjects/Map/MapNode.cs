using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public string Id;
    public MapNodeType Type;
    public RoomDataSO Room;
    public int LayerIndex;
    public int SectionIndex;
    public Vector2 Position;

    public List<MapNode> NextNodes = new List<MapNode>();
    public List<MapNode> PreviousNodes = new List<MapNode>();

    public bool Visited = false;
    public bool IsAvailable = false;
}