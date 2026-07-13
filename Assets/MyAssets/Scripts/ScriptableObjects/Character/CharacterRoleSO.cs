using UnityEngine;

[CreateAssetMenu(fileName = "CharacterRole_", menuName = "SlotGame/Party/CharacterRole")]
public class CharacterRoleSO : ScriptableObject
{
    public string RoleName;
    public CharacterRole Role;
    public string RoleDescription;
}
