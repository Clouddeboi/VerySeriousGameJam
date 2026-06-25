using System.Collections.Generic;
using UnityEngine;

public class RoomSystem : MonoBehaviour
{
    public CombatStateMachine Combat;
    public RewardSystem Rewards;
    public ShopSystem Shop;
    public EnemySpawner Spawner;
    public RoomLayoutController LayoutController;
    public TransitionController Transition;

    public System.Action OnRoomComplete;
    private System.Action pendingRoomComplete;

    public void EnterRoom(RoomDataSO room, System.Action onComplete)
    {
        Transition.PlayTransition(() =>
        {
            LayoutController.ApplyLayout(room.Layout);

            if (room.Type == RoomType.Combat || room.Type == RoomType.Elite || room.Type == RoomType.Boss)
            {
                var enemies = Spawner.SpawnForRoom(room);
                Combat.StartCombat(enemies);
                pendingRoomComplete = onComplete;
            }
            else if (room.Type == RoomType.Shop)
            {
                Shop.OpenShop(() => onComplete?.Invoke());
            }
            else
            {
                onComplete?.Invoke();
            }
        });
    }

    public void OnCombatVictory()
    {
        Rewards.PresentChoices(() =>
        {
            pendingRoomComplete?.Invoke();
            pendingRoomComplete = null;
        });
    }

}