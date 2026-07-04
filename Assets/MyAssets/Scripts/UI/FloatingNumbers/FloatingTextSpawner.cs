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

    [Header("Enemy Action Wheel Style")]
    public Color AttackActionColor = new Color(1f, 0.3f, 0.3f);
    public Color MissActionColor = Color.gray;
    public Color HealActionColor = new Color(0.3f, 1f, 0.4f);

    [Header("Synergy Text Style")]
    public Color SynergyTextColor = new Color(1f, 0.85f, 0f);
    public float SynergyTypewriterDelay = 0.04f;

    [Header("Effect Text Style")]
    public Color DebuffAppliedColor = new Color(1f, 0.4f, 0.2f);
    public Color BuffAppliedColor = new Color(0.4f, 1f, 0.5f);
    public Color EffectTickColor = new Color(1f, 0.6f, 0.2f);
    public Color InteractionColor = new Color(0.4f, 0.8f, 1f);

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

    public void SpawnActionResultText(EnemyActionType action, Vector3 worldPosition)
    {
        string label = action switch
        {
            EnemyActionType.Attack => "Attack!",
            EnemyActionType.Miss => "Miss!",
            EnemyActionType.Heal => "Heal!",
            _ => action.ToString()
        };

        Color color = action switch
        {
            EnemyActionType.Attack => AttackActionColor,
            EnemyActionType.Miss => MissActionColor,
            EnemyActionType.Heal => HealActionColor,
            _ => Color.white
        };

        var instance = Instantiate(FloatingTextPrefab, worldPosition, Quaternion.identity);
        instance.TypewriterCharDelay = 0f; 
        instance.Play(label, color);
    }

    public void SpawnSynergyText(string content, Vector3 worldPosition)
    {
        var instance = Instantiate(FloatingTextPrefab, worldPosition, Quaternion.identity);
        instance.TypewriterCharDelay = SynergyTypewriterDelay;
        instance.Play(content, SynergyTextColor);
    }

    public void SpawnEffectAppliedText(string effectName, bool isBuff, Vector3 worldPosition)
    {
        var instance = Instantiate(FloatingTextPrefab, worldPosition, Quaternion.identity);
        instance.TypewriterCharDelay = 0f;
        instance.Play(effectName, isBuff ? BuffAppliedColor : DebuffAppliedColor);
    }

    public void SpawnEffectTickText(string effectName, Vector3 worldPosition)
    {
        var instance = Instantiate(FloatingTextPrefab, worldPosition, Quaternion.identity);
        instance.TypewriterCharDelay = 0f;
        instance.Play(effectName, EffectTickColor);
    }

    public void SpawnInteractionText(string message, Vector3 worldPosition)
    {
        var instance = Instantiate(FloatingTextPrefab, worldPosition, Quaternion.identity);
        instance.TypewriterCharDelay = 0.03f;
        instance.Play(message, InteractionColor);
    }
}