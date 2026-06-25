using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public FloatingText FloatingTextPrefab;

    [Header("Slot Result Text Style")]
    public Color SlotResultColor = Color.white;
    public float SlotResultTypewriterDelay = 0.03f;

    [Header("Damage Number Style")]
    public Color DamageColor = Color.red;
    public Color HealColor = Color.green;

    public void SpawnSlotResultText(string content, Vector3 worldPosition)
    {
        var instance = Instantiate(FloatingTextPrefab, worldPosition, Quaternion.identity);
        instance.TypewriterCharDelay = SlotResultTypewriterDelay;
        instance.Play(content, SlotResultColor);
    }

    public void SpawnDamageNumber(int amount, Vector3 worldPosition)
    {
        var instance = Instantiate(FloatingTextPrefab, worldPosition, Quaternion.identity);
        instance.TypewriterCharDelay = 0f;
        instance.Play("-" + amount.ToString(), DamageColor);
    }

    public void SpawnHealNumber(int amount, Vector3 worldPosition)
    {
        var instance = Instantiate(FloatingTextPrefab, worldPosition, Quaternion.identity);
        instance.TypewriterCharDelay = 0f;
        instance.Play($"+{amount}", HealColor);
    }
}