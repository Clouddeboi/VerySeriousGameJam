using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "SlotGame/Room/RoomData")]
public class RoomDataSO : ScriptableObject
{
    public RoomType Type;
    public EnemyData[] PossibleEnemies;
}