using UnityEngine;

public class SystemTest : MonoBehaviour
{
    public RunManager RunManager;
    public CombatStateMachine Combat;

    private void Start()
    {
        RunManager.StartRun();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Combat.PlayerSpinAndAttack();
        }
    }
}