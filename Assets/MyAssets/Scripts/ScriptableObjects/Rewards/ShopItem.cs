using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem_", menuName = "SlotGame/Shop/ShopItem")]
public class ShopItemSO : ScriptableObject
{
    public RewardData Reward;
    public int Price;
}