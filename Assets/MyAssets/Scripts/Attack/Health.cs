using UnityEngine;

public class Health : MonoBehaviour
{
    public string EntityName = "Entity";
    public int MaxHP = 30;
    public int CurrentHP { get; private set; }
    public bool IsDead => CurrentHP <= 0;

    private void Awake()
    {
        CurrentHP = MaxHP;
    }

    public void ApplyDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Debug.Log($"[Health] {EntityName} took {amount} dmg -> {CurrentHP}/{MaxHP} HP");

        if (IsDead)
            Debug.Log($"[Health] {EntityName} died.");
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        Debug.Log($"[Health] {EntityName} healed {amount} -> {CurrentHP}/{MaxHP} HP");
    }

    public void SetMaxHP(int newMax)
    {
        MaxHP = newMax;
        CurrentHP = newMax;
    }
}