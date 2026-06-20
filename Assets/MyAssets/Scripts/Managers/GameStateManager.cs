using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Boot;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetState(GameState newState)
    {
        Debug.Log($"[GameState] {CurrentState} -> {newState}");
        CurrentState = newState;
    }
}