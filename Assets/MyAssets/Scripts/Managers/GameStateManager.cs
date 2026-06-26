using UnityEngine;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Boot;
    public GameOverReason LastGameOverReason { get; private set; }
    public event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetState(GameState newState)
    {
        Debug.Log($"[GameState] {CurrentState} -> {newState}");
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    public void SetGameOver(GameOverReason reason)
    {
        LastGameOverReason = reason;
        SetState(GameState.GameOver);
    }
}