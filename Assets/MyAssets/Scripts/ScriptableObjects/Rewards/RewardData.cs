using UnityEngine;

public enum RewardType { Gold, WeightAdjustment, Heal }

[CreateAssetMenu(fileName = "Reward_", menuName = "SlotGame/Reward/RewardData")]
public class RewardData : ScriptableObject
{
    public string DisplayName;
    public RewardType Type;
    public Sprite Icon;

    [Header("Gold")]
    public int GoldAmount;

    [Header("Weight Adjustment")]
    public WeaponSymbol TargetWeapon;
    public ElementSymbol TargetElement;
    public float WeightDelta;

    [Header("Heal")]
    public int HealAmount;
}