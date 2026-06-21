using UnityEngine;
using TMPro;

public class CombatUI : MonoBehaviour
{
    public Health PlayerHealth;
    public Health EnemyHealth;

    public TMP_Text PlayerHPText;
    public TMP_Text EnemyHPText;
    public TMP_Text TurnStateText;

    public CombatStateMachine Combat;

    private void Update()
    {
        PlayerHPText.text = $"{PlayerHealth.CurrentHP}/{PlayerHealth.MaxHP}";
        EnemyHPText.text = $"{EnemyHealth.CurrentHP}/{EnemyHealth.MaxHP}";
        TurnStateText.text = Combat.CurrentState.ToString();
    }
}