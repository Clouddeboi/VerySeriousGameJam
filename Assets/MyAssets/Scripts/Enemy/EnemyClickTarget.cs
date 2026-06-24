using UnityEngine;

public class EnemyClickTarget : MonoBehaviour
{
    public EnemyAI Enemy;
    public CombatStateMachine Combat;
    public GameObject HoverIndicator;

    private void Awake()
    {
        if (HoverIndicator != null) HoverIndicator.SetActive(false);
    }

    private void Start()
    {
        Enemy.Health.OnDeath += () =>
        {
            if (HoverIndicator != null) HoverIndicator.SetActive(false);
        };
    }

    private void OnMouseDown()
    {
        Combat.SelectTarget(Enemy);
    }

    private void OnMouseEnter()
    {
        if (Enemy.Health.IsDead) return;
        if (HoverIndicator != null) HoverIndicator.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (HoverIndicator != null) HoverIndicator.SetActive(false);
    }
}