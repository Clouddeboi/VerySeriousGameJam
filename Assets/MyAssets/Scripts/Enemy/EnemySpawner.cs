using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab; //A prefab with EnemyAI + Health + SpriteRenderer already wired
    public Transform EnemyContainer; //The GameObject all spawned enemies become children of
    public CombatStateMachine Combat;
    public StatusEffectSystem StatusEffects;

    private List<EnemyAI> spawned = new List<EnemyAI>();

    public List<EnemyAI> SpawnForRoom(RoomDataSO room)
    {
        ClearCurrent();

        int count = room.EnemyCount;

        for (int i = 0; i < count; i++)
        {
            EnemyData data = room.PossibleEnemies[Random.Range(0, room.PossibleEnemies.Length)];

            GameObject go = Instantiate(EnemyPrefab, EnemyContainer);
            go.transform.localPosition = GetSlotPosition(i, count);

            EnemyAI ai = go.GetComponent<EnemyAI>();
            ai.Initialize(data);

            EnemyClickTarget clickTarget = go.GetComponent<EnemyClickTarget>();
            clickTarget.Enemy = ai;
            clickTarget.Combat = Combat;

            StatusEffectVisual statusVisual = go.GetComponent<StatusEffectVisual>();
            if (statusVisual != null)
            {
                statusVisual.StatusEffects = StatusEffects;
                statusVisual.Target = ai.Health;
                statusVisual.Initialize();
            }
            
            spawned.Add(ai);
        }

        Debug.Log($"[Spawner] Spawned {spawned.Count} enemies for {room.Type} room.");
        return spawned;
    }

    //Simple horizontal layout
    private Vector3 GetSlotPosition(int index, int total)
    {
        float spacing = 2.5f;
        float startX = -(total - 1) * spacing / 2f;
        return new Vector3(startX + index * spacing, 0f, 0f);
    }

    public void ClearCurrent()
    {
        foreach (var e in spawned)
        {
            if (e != null) Destroy(e.gameObject);
        }
        spawned.Clear();
    }

    public void RemoveDead(EnemyAI enemy)
    {
        spawned.Remove(enemy);
    }
}