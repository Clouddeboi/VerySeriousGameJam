using UnityEngine;

public class RoomLayoutController : MonoBehaviour
{
    public RectTransform SlotMachineRoot;
    public Transform EnemyContainer;
    public GameObject ShopPanelRoot;
    public Transform PlayerTransform;
    public SpriteRenderer BackgroundRenderer;

    public void ApplyLayout(RoomLayoutPreset layout)
    {
        SlotMachineRoot.gameObject.SetActive(layout.SlotMachineVisible);
        SlotMachineRoot.anchoredPosition = layout.SlotMachinePosition;

        EnemyContainer.gameObject.SetActive(layout.EnemyContainerVisible);
        EnemyContainer.position = layout.EnemyContainerPosition;

        ShopPanelRoot.SetActive(layout.ShopPanelVisible);

        PlayerTransform.position = layout.PlayerPosition;

        if (layout.BackgroundSprite != null && BackgroundRenderer != null)
            BackgroundRenderer.sprite = layout.BackgroundSprite;

        Debug.Log($"[Layout] Applied layout: {layout.name}");
    }
}