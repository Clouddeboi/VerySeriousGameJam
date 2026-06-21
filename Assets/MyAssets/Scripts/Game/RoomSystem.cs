using System;
using UnityEngine;

public class RoomSystem : MonoBehaviour
{
    public CombatStateMachine Combat;
    public RewardSystem Rewards;
    public EnemySpawner Spawner;

    public RoomState CurrentRoomState { get; private set; }
    public System.Action OnRoomComplete;

    public void EnterRoom(RoomDataSO room)
    {
        CurrentRoomState = RoomState.Enter;
        Debug.Log($"[Room] Entering {room.Type} room.");

        if (room.Type == RoomType.Combat || room.Type == RoomType.Elite || room.Type == RoomType.Boss)
        {
            var enemies = Spawner.SpawnForRoom(room);

            CurrentRoomState = RoomState.Combat;
            Combat.StartCombat(enemies);
        }
        else
        {
            Debug.Log($"[Room] {room.Type} room not implemented yet — skipping to Exit.");
            CurrentRoomState = RoomState.Exit;
            OnRoomComplete?.Invoke();
        }
    }

    public void OnCombatVictory()
    {
        CurrentRoomState = RoomState.Rewards;
        var choices = Rewards.GenerateChoices();
        Debug.Log($"[Room] Choose a reward (auto-picking choice 0): {choices[0].DisplayName}");
        Rewards.ApplyReward(choices[0]);

        CurrentRoomState = RoomState.Exit;
        OnRoomComplete?.Invoke();
    }
}