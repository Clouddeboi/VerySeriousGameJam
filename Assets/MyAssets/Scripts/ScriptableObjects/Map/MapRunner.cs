using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MapRunner : MonoBehaviour
{
    public MapGenerator Generator;
    public RoomSystem RoomSystem;
    public Health PlayerHealth;
    public GameStateManager GameStateManager;
    public MapUI UI;

    private List<List<MapNode>> layers;
    private MapNode bossNode;
    private MapNode currentNode;

    public void StartRun()
    {
        layers = Generator.GenerateMap(out _, out bossNode);
        currentNode = null;

        Debug.Log("[Map] Run started. New map generated.");
        GameStateManager.SetState(GameState.Map);
        UI.DisplayMap(layers);
    }

    public void TrySelectNode(MapNode node)
    {
        if (!node.IsAvailable)
        {
            Debug.Log("[Map] That node isn't reachable yet.");
            return;
        }

        EnterNode(node);
    }

    private void EnterNode(MapNode node)
    {
        node.Visited = true;
        currentNode = node;

        LockSiblingsAndPast(node);

        foreach (var next in node.NextNodes)
            next.IsAvailable = true;

        UI.RefreshAvailability();
        UI.HideMap();

        GameStateManager.SetState(GameState.Combat);

        RoomSystem.EnterRoom(node.Room, () => OnRoomFinished(node));
    }

    private void OnRoomFinished(MapNode node)
    {
        if (PlayerHealth.IsDead)
        {
            Debug.Log("[Map] Player died. Run over.");
            GameStateManager.SetState(GameState.GameOver);
            return;
        }

        if (node.Type == MapNodeType.Boss)
        {
            Debug.Log("[Map] Boss defeated! Run won!");
            GameStateManager.SetState(GameState.GameOver);
            return;
        }

        GameStateManager.SetState(GameState.Map);
        UI.ShowMap();
        UI.RefreshAvailability();
    }

    private void LockSiblingsAndPast(MapNode chosen)
    {
        foreach (var layer in layers)
        {
            foreach (var node in layer)
            {
                if (node == chosen) continue;
                if (node.Visited) continue;
                node.IsAvailable = false;
            }
        }
    }
}