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

    public void EnterRoom(RoomDataSO room)
    {
        Transition.PlayTransition(() =>
        {
            LayoutController.ApplyLayout(room.Layout);

            if (room.Type == RoomType.Combat || room.Type == RoomType.Elite || room.Type == RoomType.Boss)
            {
                var enemies = Spawner.SpawnForRoom(room);
                Combat.StartCombat(enemies);
            }
            else if (room.Type == RoomType.Shop)
            {
                Shop.OpenShop(() => OnRoomComplete?.Invoke());
            }
            else
            {
                OnRoomComplete?.Invoke();
            }
        });
    }

    public void OnCombatVictory()
    {
        Rewards.PresentChoices(() =>
        {
            OnRoomComplete?.Invoke();
        });
    }
}