using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CombatStateMachine : MonoBehaviour
{
    public SlotMachineSystem SlotMachine;
    public AttackExecutionSystem AttackExecution;
    public RoomSystem RoomSystem;
    public JackpotDetectionSystem JackpotSystem;
    public SlotMachineUI SlotUI;
    public EnemyActionWheelVisual ActionWheel;
    public FloatingTextSpawner TextSpawner;
    public GameStateManager GameStateManager;
    public BuffDebuffSystem BuffDebuff;
    public DamageSystem DamageSystem;
    public SynergyResolver SynergyResolver;
    public PartyPassiveHandler PassiveHandler;
    public EnemyAIExecutor EnemyExecutor;

    public List<EnemyAI> Enemies = new List<EnemyAI>();
    public EnemyAI CurrentTarget { get; private set; }

    private int currentPartyTurnIndex = 0;

    private PartyMember CurrentPartyMember
    {
        get
        {
            var members = PartyManager.Instance.ActiveMembers;
            if (currentPartyTurnIndex >= 0 && currentPartyTurnIndex < members.Count)
                return members[currentPartyTurnIndex];
            return null;
        }
    }

    [Header("Turn Pacing")]
    public float DelayBeforeEnemyTurn = 0.6f;
    public float DelayBetweenEnemyAttacks = 0.4f;

    public CombatState CurrentState { get; private set; }

    public void StartCombat(List<EnemyAI> enemies)
    {
        Enemies = enemies;
        currentPartyTurnIndex = 0;
        SkipDeadPartyMembers();

        CurrentTarget = null;

        LoadCurrentPartyMemberReels();
        SlotUI.RefreshLockIcons();
        SlotUI.ResetReelsToIdle();

        PassiveHandler.HandleCombatStart(enemies);

        CurrentState = CombatState.SelectingTarget;
        Debug.Log($"[Combat] Combat started. {CurrentPartyMember?.Data.CharacterName}'s turn — select a target.");
    }

    public void SelectTarget(EnemyAI enemy)
    {
        if (CurrentState != CombatState.SelectingTarget && CurrentState != CombatState.PlayerTurn) return;
        if (enemy.Health.IsDead) return;

        if (CurrentTarget == enemy)
        {
            CurrentTarget = null;
            CurrentState = CombatState.SelectingTarget;
            Debug.Log("[Combat] Target deselected.");
            return;
        }

        CurrentTarget = enemy;
        CurrentState = CombatState.PlayerTurn;
        Debug.Log($"[Combat] {CurrentPartyMember?.Data.CharacterName} targeting {enemy.Data.EnemyName}");
    }

    public void PlayerSpinAndAttack()
    {
        if (CurrentState != CombatState.PlayerTurn || CurrentTarget == null) return;
        if (CurrentPartyMember == null || CurrentPartyMember.Health.IsDead)
        {
            AdvancePartyTurn();
            return;
        }

        CurrentState = CombatState.ResolvingPlayerAttack;
        SlotResult result = SlotMachine.Spin();

        SlotUI.PlaySpinAnimation(result, SlotMachine.IsWeaponLocked, () =>
        {
            var synergies = SynergyResolver.Resolve(result);

            bool isCrit = result.Modifier != null && result.Modifier.IsCrit;
            ElementType element = result.Element?.Type ?? ElementType.None;

            var passiveBonuses = PassiveHandler.EvaluateOffensivePassives(
                CurrentPartyMember, CurrentTarget, element, isCrit);

            Attack attack = AttackBuilder.Build(result, synergies,
                passiveBonuses, CurrentPartyMember?.Data);
            attack.Source = CurrentPartyMember?.Health;

            JackpotType jackpot = JackpotSystem.Detect(result);
            JackpotSystem.TriggerVisuals(jackpot);

            if (synergies.MatchedSynergies.Count > 0)
            {
                for (int i = 0; i < synergies.MatchedSynergies.Count; i++)
                {
                    string displayText = synergies.MatchedSynergies[i].GetDisplayText(result);
                    TextSpawner.SpawnSynergyText(displayText,
                        SlotUI.SlotResultTextSpawnPoint.position + Vector3.up * i * 0.5f);
                }
            }
            else
            {
                string combo = $"{result.Weapon?.DisplayName} " +
                    $"{result.Element?.DisplayName} {result.Modifier?.DisplayName}";
                TextSpawner.SpawnSlotResultText(combo, SlotUI.SlotResultTextSpawnPoint.position);
            }

            foreach (var debuff in passiveBonuses.DebuffsToApplyToTarget)
                BuffDebuff.ApplyEffect(CurrentTarget.Health, debuff);

            foreach (var buff in passiveBonuses.BuffsToApplyToSelf)
                BuffDebuff.ApplyEffect(CurrentPartyMember.Health, buff);

            CurrentPartyMember?.PlayAttackAnimation();
            AudioManager.Instance.PlayPlayerAttack();
            AttackExecution.Execute(attack, CurrentTarget.Health,
                synergies, Enemies, CurrentTarget.Data, OnPlayerAttackResolved);
        });
    }

    private void OnPlayerAttackResolved()
    {
        if (CurrentTarget != null && CurrentTarget.Health.IsDead)
            HandleEnemyDeath(CurrentTarget);

        if (Enemies.TrueForAll(e => e == null || e.Health.IsDead))
        {
            CurrentState = CombatState.Victory;
            Debug.Log("[Combat] Victory!");
            RoomSystem.OnCombatVictory();
            return;
        }

        AdvancePartyTurn();
    }

    private void AdvancePartyTurn()
    {
        currentPartyTurnIndex++;
        SkipDeadPartyMembers();

        if (currentPartyTurnIndex >= PartyManager.Instance.ActiveMembers.Count)
        {
            currentPartyTurnIndex = 0;
            CurrentState = CombatState.EnemyTurn;
            StartCoroutine(EnemyTurnWithDelay());
        }
        else
        {
            CurrentTarget = null;
            LoadCurrentPartyMemberReels();
            SlotUI.RefreshLockIcons();
            SlotUI.ResetReelsToIdle();
            CurrentState = CombatState.SelectingTarget;
            Debug.Log($"[Combat] {CurrentPartyMember?.Data.CharacterName}'s turn — select a target.");
        }
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

            if (BuffDebuff.IsActionPrevented(enemy.Health))
            {
                Debug.Log($"[Combat] {enemy.Data.EnemyName} is frozen and can't attack!");
                BuffDebuff.TickEffects(enemy.Health);
                yield return new WaitForSeconds(DelayBetweenEnemyAttacks);
                continue;
            }

            ActionWheel.transform.position = enemy.transform.position + Vector3.down * 1f;
            ActionWheel.Setup(enemy.Data);

            EnemyActionType chosenAction = EnemyActionWheel.PickAction(enemy.Data);
            var slices = EnemyActionWheel.BuildSlices(enemy.Data);
            float landingAngle = EnemyActionWheel.GetLandingAngle(slices, chosenAction);

            bool wheelDone = false;
            ActionWheel.SpinAndLand(landingAngle, () => wheelDone = true);
            AudioManager.Instance.PlayEnemyWheelSpin();
            yield return new WaitUntil(() => wheelDone);

            TextSpawner.SpawnActionResultText(chosenAction,
                ActionWheel.transform.position + Vector3.up * 0.5f);

            EnemyExecutor.ExecuteAction(enemy, chosenAction, Enemies);
            BuffDebuff.TickEffects(enemy.Health);

            yield return new WaitForSeconds(DelayBetweenEnemyAttacks);

            if (!PartyManager.Instance.IsPartyAlive())
            {
                CurrentState = CombatState.Defeat;
                Debug.Log("[Combat] Defeat...");
                GameStateManager.SetGameOver(GameOverReason.Defeat);
                yield break;
            }
        }

        foreach (var member in PartyManager.Instance.ActiveMembers)
            if (member != null && !member.Health.IsDead)
                BuffDebuff.TickEffects(member.Health);

        if (Enemies.TrueForAll(e => e == null || e.Health.IsDead))
        {
            CurrentState = CombatState.Victory;
            Debug.Log("[Combat] Victory! (DoT kill)");
            RoomSystem.OnCombatVictory();
            yield break;
        }

        currentPartyTurnIndex = 0;
        SkipDeadPartyMembers();

        CurrentTarget = null;
        LoadCurrentPartyMemberReels();
        SlotUI.RefreshLockIcons();
        SlotUI.ResetReelsToIdle();
        CurrentState = CombatState.SelectingTarget;
        Debug.Log($"[Combat] New round. {CurrentPartyMember?.Data.CharacterName}'s turn — select a target.");
    }

    private void SkipDeadPartyMembers()
    {
        var members = PartyManager.Instance.ActiveMembers;
        while (currentPartyTurnIndex < members.Count)
        {
            var m = members[currentPartyTurnIndex];
            if (m != null && !m.Health.IsDead) break;
            currentPartyTurnIndex++;
        }
    }

    private void LoadCurrentPartyMemberReels()
    {
        SlotMachine.ResetAllLocks();
        SlotMachine.LoadCharacter(CurrentPartyMember?.Data);
    }

    private void HandleEnemyDeath(EnemyAI enemy)
    {
        Debug.Log($"[Combat] {enemy.Data.EnemyName} died.");
    }
}