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

    public ScrollRect MapScrollRect;

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

        float maxX = 0f;
        float minY = 0f;
        float maxY = 0f;

        foreach (var layer in layers)
        {
            foreach (var node in layer)
            {
                var buttonGO = Instantiate(NodeButtonPrefab, MapContainer);
                var button = buttonGO.GetComponent<MapNodeButton>();

                Vector2 pos = new Vector2(node.Position.x * HorizontalSpacing, node.Position.y * VerticalSpacing);
                buttonGO.GetComponent<RectTransform>().anchoredPosition = pos;

                maxX = Mathf.Max(maxX, Mathf.Abs(pos.x));
                minY = Mathf.Min(minY, pos.y);
                maxY = Mathf.Max(maxY, pos.y);

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

        float padding = 200f;
        MapContainer.sizeDelta = new Vector2(maxX * 2f + padding, (maxY - minY) + padding);

        RefreshAvailability();
        ShowMap();
        ScrollToNode(currentLayers[0][0]);
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
        Debug.Log($"[MapUI] RefreshAvailability called, {nodeButtons.Count} buttons");
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

    private void ScrollToNode(MapNode node)
    {
        if (MapScrollRect == null) return;

        float targetX = node.Position.x * HorizontalSpacing;
        float containerWidth = MapContainer.sizeDelta.x;
        float viewportWidth = MapScrollRect.viewport.rect.width;

        float normalizedX = Mathf.Clamp01((targetX + containerWidth / 2f) / containerWidth);
        MapScrollRect.horizontalNormalizedPosition = normalizedX;
    }
}