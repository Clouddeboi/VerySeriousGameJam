using UnityEngine;

public class SystemTest : MonoBehaviour
{
    public RunManager RunManager;
    public CombatStateMachine Combat;
    public MapRunner MapRunner;

    private void Start()
    {
        MapRunner.StartRun();
    }

    private void Update()
    {

    }
}