using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public GameObject PanelRoot;
    public GameObject NodeButtonPrefab;
    public RectTransform MapContainer;
    public MapRunner Runner;

    [Header("Layout")]
    public float HorizontalSpacing = 180f;
    public float VerticalSpacing = 120f;

    [Header("Node Type Colors/Icons")]
    public Sprite CombatIcon;
    public Sprite EliteIcon;
    public Sprite ShopIcon;
    public Sprite BossIcon;

    private Dictionary<MapNode, MapNodeButton> nodeButtons = new Dictionary<MapNode, MapNodeButton>();
    private List<List<MapNode>> currentLayers;

    public void DisplayMap(List<List<MapNode>> layers)
    {
        currentLayers = layers;
        ClearExisting();

        foreach (var layer in layers)
        {
            foreach (var node in layer)
            {
                var buttonGO = Instantiate(NodeButtonPrefab, MapContainer);
                var button = buttonGO.GetComponent<MapNodeButton>();

                Vector2 pos = new Vector2(node.Position.x * HorizontalSpacing, node.Position.y * VerticalSpacing);
                buttonGO.GetComponent<RectTransform>().anchoredPosition = pos;

                Sprite icon = node.Type switch
                {
                    MapNodeType.Combat => CombatIcon,
                    MapNodeType.Elite => EliteIcon,
                    MapNodeType.Shop => ShopIcon,
                    MapNodeType.Boss => BossIcon,
                    _ => null
                };

                button.Setup(node, icon, () => Runner.TrySelectNode(node));
                nodeButtons[node] = button;
            }
        }

        DrawConnections();
        RefreshAvailability();
        ShowMap();
    }

    private void DrawConnections()
    {
        foreach (var layer in currentLayers)
        {
            foreach (var node in layer)
            {
                foreach (var next in node.NextNodes)
                {
                    CreateConnectionLine(nodeButtons[node].GetComponent<RectTransform>().anchoredPosition,
                                          nodeButtons[next].GetComponent<RectTransform>().anchoredPosition);
                }
            }
        }
    }

    private void CreateConnectionLine(Vector2 from, Vector2 to)
    {
        var lineGO = new GameObject("ConnectionLine", typeof(RectTransform), typeof(Image));
        lineGO.transform.SetParent(MapContainer, false);
        lineGO.transform.SetAsFirstSibling();

        var rt = lineGO.GetComponent<RectTransform>();
        var image = lineGO.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.4f);

        Vector2 dir = to - from;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        rt.sizeDelta = new Vector2(distance, 4f);
        rt.anchoredPosition = from + dir * 0.5f;
        rt.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void RefreshAvailability()
    {
        foreach (var kvp in nodeButtons)
            kvp.Value.SetAvailability(kvp.Key.IsAvailable, kvp.Key.Visited);
    }

    public void ShowMap() => PanelRoot.SetActive(true);
    public void HideMap() => PanelRoot.SetActive(false);

    private void ClearExisting()
    {
        foreach (Transform child in MapContainer)
            Destroy(child.gameObject);
        nodeButtons.Clear();
    }
}