using UnityEngine;

[CreateAssetMenu(fileName = "Faction_", menuName = "SlotGame/Enemy/Faction")]
public class EnemyFaction : ScriptableObject
{
    public string FactionName;

    [Header("Factions this faction is hostile toward")]
    public EnemyFaction[] HostileToward;

    public bool IsHostileToward(EnemyFaction other)
    {
        if (other == null) return false;
        foreach (var f in HostileToward)
            if (f == other) return true;
        return false;
    }
}