using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
    public List<EnemyAI> Enemies = new List<EnemyAI>();
    public EnemyAI CurrentTarget { get; private set; }
    public EnemyAI Enemy;

    public CombatState CurrentState { get; private set; }

    public void StartCombat(List<EnemyAI> enemies)
    {
        Enemies = enemies;
        CurrentTarget = null;
        SlotMachine.ResetAllLocks();
        SlotUI.RefreshLockIcons();
        SlotUI.ResetReelsToIdle();
        CurrentState = CombatState.SelectingTarget;
        Debug.Log("[Combat] Combat started. Select a target.");
    }

    public void SelectTarget(EnemyAI enemy)
    {
        if (CurrentState != CombatState.SelectingTarget) return;
        if (enemy.Health.IsDead) return;

        CurrentTarget = enemy;
        CurrentState = CombatState.PlayerTurn;
        Debug.Log($"[Combat] Target selected: {enemy.Data.EnemyName}");
    }

    public void PlayerSpinAndAttack()
    {
        if (CurrentState != CombatState.PlayerTurn || CurrentTarget == null) return;

        CurrentState = CombatState.ResolvingPlayerAttack;

        SlotResult result = SlotMachine.Spin();

        SlotUI.PlaySpinAnimation(result, () =>
        {
            //This runs only after all 3 reels finish their spin animation
            Attack attack = AttackBuilder.Build(result);

            JackpotType jackpot = JackpotSystem.Detect(result);
            attack = JackpotSystem.ApplyJackpotBonus(attack, jackpot);

            AttackExecution.Execute(attack, CurrentTarget.Health, OnPlayerAttackResolved);
        });
    }

    private void OnPlayerAttackResolved()
    {
        if (CurrentTarget.Health.IsDead)
            HandleEnemyDeath(CurrentTarget);

        if (Enemies.TrueForAll(e => e == null || e.Health.IsDead))
        {
            CurrentState = CombatState.Victory;
            Debug.Log("[Combat] Victory!");
            RoomSystem.OnCombatVictory();
            return;
        }

        CurrentState = CombatState.EnemyTurn;
        StartCoroutine(RunEnemyTurnSequence());
    }

    private IEnumerator RunEnemyTurnSequence()
    {
        CurrentState = CombatState.ResolvingEnemyAttack;

        foreach (var enemy in Enemies)
        {
            if (enemy == null || enemy.Health.IsDead) continue;

            if (StatusEffects.IsFrozen(enemy.Health))
            {
                Debug.Log($"[Combat] {enemy.Data.EnemyName} is frozen and can't attack!");
            }
            else
            {
                int dmg = enemy.DecideAndGetDamage();
                PlayerHealth.ApplyDamage(dmg);
            }

            StatusEffects.TickEffects(enemy.Health);

            yield return new WaitForSeconds(0.4f);

            if (PlayerHealth.IsDead)
            {
                CurrentState = CombatState.Defeat;
                Debug.Log("[Combat] Defeat...");
                yield break;
            }
        }

        StatusEffects.TickEffects(PlayerHealth);

        //Re-check victory here too cause if an enemy may have died from a DoT tick mid-sequence, not from the player's hit
        if (Enemies.TrueForAll(e => e == null || e.Health.IsDead))
        {
            CurrentState = CombatState.Victory;
            Debug.Log("[Combat] Victory! (enemy succumbed to status effect)");
            RoomSystem.OnCombatVictory();
            yield break;
        }

        CurrentTarget = null;
        SlotMachine.ResetAllLocks();
        SlotUI.RefreshLockIcons();
        SlotUI.ResetReelsToIdle(); 
        CurrentState = CombatState.SelectingTarget;
        Debug.Log("[Combat] Select a target.");
    }

    private void HandleEnemyDeath(EnemyAI enemy)
    {
        Debug.Log($"[Combat] {enemy.Data.EnemyName} died.");
    }
}