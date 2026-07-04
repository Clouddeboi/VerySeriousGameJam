using UnityEngine;

[System.Serializable]
public class ElementalAffinityEntry
{
    public ElementType Element;
    public ElementalResponse Response;
}

[CreateAssetMenu(fileName = "EnemyType_", menuName = "SlotGame/Enemy/EnemyType")]
public class EnemyType : ScriptableObject
{
    public string TypeName;

    [Header("Default Elemental Affinities for this type")]
    public ElementalAffinityEntry[] DefaultAffinities;
}