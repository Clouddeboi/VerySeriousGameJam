using UnityEngine;

public class CombatStateMachine : MonoBehaviour
{
    public SlotMachineSystem SlotMachine;
    public AttackBuilder AttackBuilder;
    public AttackExecutionSystem AttackExecution;
    public RoomSystem RoomSystem;

    public Health PlayerHealth;
    public EnemyAI Enemy;

    public CombatState CurrentState { get; private set; }

    public void StartCombat()
    {
        CurrentState = CombatState.PlayerTurn;
        Debug.Log("[Combat] Combat started. Player's turn.");
    }

    public void PlayerSpinAndAttack()
    {
        if (CurrentState != CombatState.PlayerTurn) return;

        CurrentState = CombatState.ResolvingPlayerAttack;

        SlotResult result = SlotMachine.Spin();
        Attack attack = AttackBuilder.Build(result);

        AttackExecution.Execute(attack, Enemy.Health, OnPlayerAttackResolved);
    }

    private void OnPlayerAttackResolved()
    {
        if (Enemy.Health.IsDead)
        {
            CurrentState = CombatState.Victory;
            Debug.Log("[Combat] Victory!");
            RoomSystem.OnCombatVictory(); 
            return;
        }

        CurrentState = CombatState.EnemyTurn;
        RunEnemyTurn();
    }

    private void RunEnemyTurn()
    {
        CurrentState = CombatState.ResolvingEnemyAttack;

        int dmg = Enemy.DecideAndGetDamage();
        PlayerHealth.ApplyDamage(dmg);

        if (PlayerHealth.IsDead)
        {
            CurrentState = CombatState.Defeat;
            Debug.Log("[Combat] Defeat...");
            return;
        }

        CurrentState = CombatState.PlayerTurn;
        Debug.Log("[Combat] Player's turn.");
    }
}