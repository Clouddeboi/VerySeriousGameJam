using UnityEngine;

public class SystemTest : MonoBehaviour
{
    public RunManager RunManager;
    public CombatStateMachine Combat;
    public MapRunner MapRunner;

    private void Start()
    {
        MapRunner.StartRun();
        PartyManager.Instance.InitializeParty();
    }

    private void Update()
    {

    }
}