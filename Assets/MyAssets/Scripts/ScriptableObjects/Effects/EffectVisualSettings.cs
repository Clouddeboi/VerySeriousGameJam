using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "EffectSettings_", menuName = "SlotGame/Effects/EffectSettings")]
public class EffectVisualSettings : ScriptableObject
{
    [Header("Grid Layout")]
    public int Columns = 5;
    public int Rows = 2;
    public float CellWidth = 0.3f;
    public float CellHeight = 0.3f;
    public float CellSpacing = 0.05f;

    [Header("World Position")]
    public Vector3 WorldOffset = new Vector3(0f, 2f, 0f);

    [Header("Icon")]
    public float IconScale = 1f;
    public int IconSortingOrder = 5;

    [Header("Count Text")]
    public TMP_FontAsset Font;
    public float FontSize = 2f;
    public FontStyles FontStyle = FontStyles.Bold;
    public Color TextColor = Color.white;
    public Vector2 TextOffset = new Vector2(0.3f, -0.3f);
    public float TextScale = 0.15f;
    public int TextSortingOrder = 6;

    [Header("Category Colors")]
    public Color BuffColor = new Color(0.4f, 1f, 0.5f);
    public Color DebuffColor = new Color(1f, 0.4f, 0.4f);
    public Color NoIconColor = Color.white;
}