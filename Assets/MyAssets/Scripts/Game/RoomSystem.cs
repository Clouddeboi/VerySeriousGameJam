using System;
using UnityEngine;

public class RoomSystem : MonoBehaviour
{
    public CombatStateMachine Combat;
    public RewardSystem Rewards;
    public EnemyAI EnemySlot;

    public RoomState CurrentRoomState { get; private set; }

    public event Action OnRoomComplete;

    public void EnterRoom(RoomDataSO room)
    {
        CurrentRoomState = RoomState.Enter;
        Debug.Log($"[Room] Entering {room.Type} room.");

        if (room.Type == RoomType.Combat || room.Type == RoomType.Elite || room.Type == RoomType.Boss)
        {
            var enemyData = room.PossibleEnemies[UnityEngine.Random.Range(0, room.PossibleEnemies.Length)];
            EnemySlot.Initialize(enemyData);

            CurrentRoomState = RoomState.Combat;
            Combat.StartCombat();
        }
        else
        {
            //Shop/Event rooms stub for now
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