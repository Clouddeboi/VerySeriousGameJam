using UnityEngine;

public class RunManager : MonoBehaviour
{
    public RoomSystem RoomSystem;
    public RoomDataSO[] RoomSequence;//Linear for now
    public Health PlayerHealth;
    public GameStateManager gameStateManager;

    private int currentRoomIndex = -1;

    public void StartRun()
    {
        currentRoomIndex = -1;
        Debug.Log("[Run] Run started.");
        gameStateManager.SetState(GameState.Combat);
        AdvanceToNextRoom();
    }

    public void AdvanceToNextRoom()
    {
        currentRoomIndex++;

        if (currentRoomIndex >= RoomSequence.Length)
        {
            Debug.Log("[Run] All rooms cleared. Run won!");
            gameStateManager.SetState(GameState.GameOver);
            return;
        }

        if (PlayerHealth.IsDead)
        {
            Debug.Log("[Run] Player is dead. Run over.");
            gameStateManager.SetState(GameState.GameOver);
            return;
        }

        var room = RoomSequence[currentRoomIndex];
        RoomSystem.EnterRoom(room);
    }
}