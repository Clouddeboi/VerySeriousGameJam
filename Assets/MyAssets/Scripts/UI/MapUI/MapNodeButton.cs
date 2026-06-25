using UnityEngine;
using UnityEngine.UI;

public class MapNodeButton : MonoBehaviour
{
    public Image IconImage;
    public Image BackgroundImage;
    public Button Button;

    [Header("State Colors")]
    public Color AvailableColor = Color.white;
    public Color LockedColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);
    public Color VisitedColor = new Color(0.6f, 0.8f, 1f);

    private MapNode node;

    public void Setup(MapNode mapNode, Sprite icon, System.Action onClick)
    {
        node = mapNode;
        IconImage.sprite = icon;

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => onClick?.Invoke());
    }

    public void SetAvailability(bool available, bool visited)
    {
        Button.interactable = available;
        BackgroundImage.color = visited ? VisitedColor : (available ? AvailableColor : LockedColor);
    }
}