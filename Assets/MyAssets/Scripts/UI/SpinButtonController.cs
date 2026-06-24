using UnityEngine;
using UnityEngine.UI;

public class SpinButtonController : MonoBehaviour
{
    public Button SpinButton;
    public CombatStateMachine Combat;

    private void Update()
    {
        SpinButton.interactable = Combat.CurrentState == CombatState.PlayerTurn;
    }
}