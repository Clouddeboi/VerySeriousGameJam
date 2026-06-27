using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public Transform EnemyContainer;
    public CombatStateMachine Combat;

    [Header("Layout")]
    public float HorizontalSpacing = 2.5f; // fallback layout for counts with no custom diamond defined
    public float DiamondHorizontalOffset = 1.5f; // left/right distance from center
    public float DiamondVerticalOffset = 1.0f;   // top/bottom distance from center

    private List<EnemyAI> spawned = new List<EnemyAI>();

    public List<EnemyAI> SpawnForRoom(RoomDataSO room)
    {
        ClearCurrent();

        int count = room.EnemyCount;
        var positions = GetLayoutPositions(count);

        for (int i = 0; i < count; i++)
        {
            EnemyData data = room.PossibleEnemies[Random.Range(0, room.PossibleEnemies.Length)];

            GameObject go = Instantiate(EnemyPrefab, EnemyContainer);
            go.transform.localPosition = positions[i];
            go.transform.localScale = Vector3.one; // explicitly keep uniform scale regardless of position

            EnemyAI ai = go.GetComponent<EnemyAI>();
            ai.Initialize(data);

            EnemyClickTarget clickTarget = go.GetComponent<EnemyClickTarget>();
            clickTarget.Enemy = ai;
            clickTarget.Combat = Combat;

            spawned.Add(ai);
        }

        return spawned;
    }

    private List<Vector3> GetLayoutPositions(int count)
    {
        switch (count)
        {
            case 1:
                return new List<Vector3> { Vector3.zero };

            case 2:
                return new List<Vector3>
                {
                    new Vector3(-DiamondHorizontalOffset, 0f, 0f),
                    new Vector3(DiamondHorizontalOffset, 0f, 0f)
                };

            case 3:
                // Triangle: top, bottom-left, bottom-right
                return new List<Vector3>
                {
                    new Vector3(0f, DiamondVerticalOffset, 0f),
                    new Vector3(-DiamondHorizontalOffset, -DiamondVerticalOffset, 0f),
                    new Vector3(DiamondHorizontalOffset, -DiamondVerticalOffset, 0f)
                };

            case 4:
                // Diamond: top, left, right, bottom — matches the layout you described
                return new List<Vector3>
                {
                    new Vector3(0f, DiamondVerticalOffset, 0f),       // top
                    new Vector3(-DiamondHorizontalOffset, 0f, 0f),    // left
                    new Vector3(DiamondHorizontalOffset, 0f, 0f),     // right
                    new Vector3(0f, -DiamondVerticalOffset, 0f)       // bottom
                };

            default:
                // Fallback for counts beyond your defined layouts: simple horizontal line, same as before
                var fallback = new List<Vector3>();
                float startX = -(count - 1) * HorizontalSpacing / 2f;
                for (int i = 0; i < count; i++)
                    fallback.Add(new Vector3(startX + i * HorizontalSpacing, 0f, 0f));
                return fallback;
        }
    }

    public void ClearCurrent()
    {
        foreach (var e in spawned)
            if (e != null) Destroy(e.gameObject);
        spawned.Clear();
    }

    public void RemoveDead(EnemyAI enemy)
    {
        spawned.Remove(enemy);
    }
}