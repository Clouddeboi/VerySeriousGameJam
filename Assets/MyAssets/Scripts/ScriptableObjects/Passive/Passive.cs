using UnityEngine;

[CreateAssetMenu(fileName = "Passive_", menuName = "SlotGame/Party/Passive")]
public class PassiveSO : ScriptableObject
{
    public string PassiveName;
    public string Description;

    public PassiveTriggerType Trigger;

    [Header("Conditions — leave null to trigger on any")]
    public EnemyType RequiredEnemyType;
    public ElementType RequiredElement;
    public EffectDataSO RequiredEffect;

    [Header("Effects Applied When Triggered")]
    public PassiveEffect[] Effects;

    [Header("Immunity — checked externally, no effects needed")]
    public bool IsImmunity;
    public EnemyType ImmuneToEnemyType;
    public ElementType ImmuneToElement;
    public EffectDataSO ImmuneToEffect;

    public bool MatchesEnemyType(EnemyType type)
    {
        return RequiredEnemyType == null || RequiredEnemyType == type;
    }

    public bool MatchesElement(ElementType element)
    {
        return RequiredElement == ElementType.None || RequiredElement == element;
    }
}