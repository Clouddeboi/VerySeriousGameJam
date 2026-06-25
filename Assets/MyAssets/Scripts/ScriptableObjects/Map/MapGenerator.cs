using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MapSectionConfigSO[] Sections;
    public RoomDataSO BossRoom;

    public List<List<MapNode>> GenerateMap(out MapNode startNodeRef, out MapNode bossNodeRef)
    {
        var allLayers = new List<List<MapNode>>();
        MapNode previousLayerNodesRoot = null;
        List<MapNode> previousLayer = null;

        int globalLayerIndex = 0;

        for (int sectionIndex = 0; sectionIndex < Sections.Length; sectionIndex++)
        {
            var section = Sections[sectionIndex];

            for (int layer = 0; layer < section.LayerCount; layer++)
            {
                int nodeCount = Random.Range(section.MinNodesPerLayer, section.MaxNodesPerLayer + 1);
                var currentLayer = new List<MapNode>();

                for (int i = 0; i < nodeCount; i++)
                {
                    var node = new MapNode
                    {
                        Id = $"node_{sectionIndex}_{layer}_{i}",
                        Type = PickNodeType(section),
                        SectionIndex = sectionIndex,
                        LayerIndex = globalLayerIndex,
                        Position = new Vector2(globalLayerIndex, i - (nodeCount - 1) / 2f) // simple grid layout, refine visually later
                    };
                    node.Room = PickRoom(section, node.Type);
                    currentLayer.Add(node);
                }

                if (previousLayer != null)
                    ConnectLayers(previousLayer, currentLayer);

                allLayers.Add(currentLayer);
                previousLayer = currentLayer;
                globalLayerIndex++;
            }
        }

        var bossNode = new MapNode
        {
            Id = "node_boss",
            Type = MapNodeType.Boss,
            Room = BossRoom,
            SectionIndex = Sections.Length - 1,
            LayerIndex = globalLayerIndex,
            Position = new Vector2(globalLayerIndex, 0f)
        };

        foreach (var node in previousLayer)
        {
            node.NextNodes.Add(bossNode);
            bossNode.PreviousNodes.Add(node);
        }

        allLayers.Add(new List<MapNode> { bossNode });

        foreach (var node in allLayers[0])
            node.IsAvailable = true;

        startNodeRef = null;
        bossNodeRef = bossNode;

        return allLayers;
    }

    private void ConnectLayers(List<MapNode> from, List<MapNode> to)
    {
        var reachedInTo = new HashSet<MapNode>();

        foreach (var fromNode in from)
        {
            int connectionCount = Random.Range(1, 3);
            connectionCount = Mathf.Min(connectionCount, to.Count);

            var shuffled = new List<MapNode>(to);
            Shuffle(shuffled);

            for (int i = 0; i < connectionCount; i++)
            {
                var target = shuffled[i];
                fromNode.NextNodes.Add(target);
                target.PreviousNodes.Add(fromNode);
                reachedInTo.Add(target);
            }
        }

        foreach (var toNode in to)
        {
            if (!reachedInTo.Contains(toNode))
            {
                var randomSource = from[Random.Range(0, from.Count)];
                randomSource.NextNodes.Add(toNode);
                toNode.PreviousNodes.Add(randomSource);
            }
        }
    }

    private MapNodeType PickNodeType(MapSectionConfigSO section)
    {
        float total = section.CombatWeight + section.EliteWeight + section.ShopWeight;
        float roll = Random.Range(0f, total);

        if (roll < section.CombatWeight) return MapNodeType.Combat;
        roll -= section.CombatWeight;

        if (roll < section.EliteWeight) return MapNodeType.Elite;

        return MapNodeType.Shop;
    }

    private RoomDataSO PickRoom(MapSectionConfigSO section, MapNodeType type)
    {
        RoomDataSO[] pool = type switch
        {
            MapNodeType.Combat => section.CombatRooms,
            MapNodeType.Elite => section.EliteRooms,
            MapNodeType.Shop => section.ShopRooms,
            _ => null
        };

        if (pool == null || pool.Length == 0) return null;
        return pool[Random.Range(0, pool.Length)];
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}