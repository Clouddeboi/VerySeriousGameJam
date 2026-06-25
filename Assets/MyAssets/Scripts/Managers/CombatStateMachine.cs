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
    public PlayerCombatVisuals PlayerVisuals;
    public EnemyActionWheelVisual ActionWheel;
    public FloatingTextSpawner TextSpawner;

    public Health PlayerHealth;
    public List<EnemyAI> Enemies = new List<EnemyAI>();
    public EnemyAI CurrentTarget { get; private set; }
    public EnemyAI Enemy;

    [Header("Turn Pacing")]
    public float DelayBeforeEnemyTurn = 0.6f;
    public float DelayBetweenEnemyAttacks = 0.4f; 

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
        //Allow selecting OR switching target any time before the spin commits
        if (CurrentState != CombatState.SelectingTarget && CurrentState != CombatState.PlayerTurn) return;
        if (enemy.Health.IsDead) return;

        if (CurrentTarget == enemy)
        {
            //Clicking the already-selected enemy again deselects it
            CurrentTarget = null;
            CurrentState = CombatState.SelectingTarget;
            Debug.Log("[Combat] Target deselected.");
            return;
        }

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
            Attack attack = AttackBuilder.Build(result);

            JackpotType jackpot = JackpotSystem.Detect(result);
            attack = JackpotSystem.ApplyJackpotBonus(attack, jackpot);

            PlayerVisuals.PlayAttackAnimation();
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
        StartCoroutine(EnemyTurnWithDelay());
    }

    private IEnumerator EnemyTurnWithDelay()
    {
        yield return new WaitForSeconds(DelayBeforeEnemyTurn);
        yield return RunEnemyTurnSequence();
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
                StatusEffects.TickEffects(enemy.Health);
                yield return new WaitForSeconds(DelayBetweenEnemyAttacks);
                continue;
            }

            ActionWheel.transform.position = enemy.transform.position + Vector3.down * 1f;
            ActionWheel.Setup(enemy.Data);

            EnemyActionType chosenAction = EnemyActionWheel.PickAction(enemy.Data);
            var slices = EnemyActionWheel.BuildSlices(enemy.Data);
            float landingAngle = EnemyActionWheel.GetLandingAngle(slices, chosenAction);

            Debug.Log($"[Wheel] Chosen action: {chosenAction}, landing angle: {landingAngle}");

            bool wheelDone = false;
            ActionWheel.SpinAndLand(landingAngle, () => wheelDone = true);

            yield return new WaitUntil(() => wheelDone);

            TextSpawner.SpawnActionResultText(chosenAction, ActionWheel.transform.position + Vector3.up * 0.5f);

            switch (chosenAction)
            {
                case EnemyActionType.Attack:
                    Debug.Log($"[EnemyAI] {enemy.Data.EnemyName} attacks for {enemy.Data.AttackDamage}");
                    enemy.PlayAttackAnimationOnly();
                    PlayerHealth.ApplyDamage(enemy.Data.AttackDamage);
                    break;

                case EnemyActionType.Miss:
                    Debug.Log($"[EnemyAI] {enemy.Data.EnemyName} missed!");
                    break;

                case EnemyActionType.Heal:
                    Debug.Log($"[EnemyAI] {enemy.Data.EnemyName} heals for {enemy.Data.HealAmount}");
                    enemy.Health.Heal(enemy.Data.HealAmount);
                    break;
            }

            StatusEffects.TickEffects(enemy.Health);

            yield return new WaitForSeconds(DelayBetweenEnemyAttacks);

            if (PlayerHealth.IsDead)
            {
                CurrentState = CombatState.Defeat;
                Debug.Log("[Combat] Defeat...");
                yield break;
            }
        }

        StatusEffects.TickEffects(PlayerHealth);

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