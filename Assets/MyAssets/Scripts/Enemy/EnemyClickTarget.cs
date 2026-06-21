using UnityEngine;

public class EnemyClickTarget : MonoBehaviour
{
    public EnemyAI Enemy;
    public CombatStateMachine Combat;

    private void OnMouseDown()
    {
        Combat.SelectTarget(Enemy);
    }
}