using UnityEngine;

public class SystemTest : MonoBehaviour
{
    public CombatStateMachine Combat;

    private void Start()
    {
        Combat.StartCombat();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Combat.PlayerSpinAndAttack();
        }
    }
}