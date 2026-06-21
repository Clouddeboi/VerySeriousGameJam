using UnityEngine;

public class CombatStateMachine : MonoBehaviour
{
    public SlotMachineSystem SlotMachine;
    public AttackBuilder AttackBuilder;
    public AttackExecutionSystem AttackExecution;
    public RoomSystem RoomSystem;
    public StatusEffectSystem StatusEffects;
    public JackpotDetectionSystem JackpotSystem;
    public SlotMachineUI SlotUI;

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
        SlotUI.RefreshDisplay(result);
        
        Attack attack = AttackBuilder.Build(result);

        JackpotType jackpot = JackpotSystem.Detect(result);
        attack = JackpotSystem.ApplyJackpotBonus(attack, jackpot);

        AttackExecution.Execute(attack, Enemy.Health, OnPlayerAttackResolved);
    }

    private void OnPlayerAttackResolved()
    {
        if (Enemy.Health.IsDead)
        {
            CurrentState = CombatState.Victory;
            RoomSystem.OnCombatVictory();
            return;
        }

        CurrentState = CombatState.EnemyTurn;
        RunEnemyTurn();
    }
    private void RunEnemyTurn()
    {
        CurrentState = CombatState.ResolvingEnemyAttack;

        if (StatusEffects.IsFrozen(Enemy.Health))
        {
            Debug.Log($"[Combat] {Enemy.Data.EnemyName} is frozen and can't attack!");
        }
        else
        {
            int dmg = Enemy.DecideAndGetDamage();
            PlayerHealth.ApplyDamage(dmg);

            if (PlayerHealth.IsDead)
            {
                CurrentState = CombatState.Defeat;
                return;
            }
        }

        StatusEffects.TickEffects(PlayerHealth);
        StatusEffects.TickEffects(Enemy.Health);//Freeze counts down here regardless of whether they attacked

        CurrentState = CombatState.PlayerTurn;
    }
}