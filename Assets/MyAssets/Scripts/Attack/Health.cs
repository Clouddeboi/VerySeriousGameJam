using UnityEngine;

public class Health : MonoBehaviour
{
    public string EntityName = "Entity";
    public int MaxHP = 30;
    public int CurrentHP { get; private set; }
    public bool IsDead => CurrentHP <= 0;

    public System.Action OnDeath;
    public System.Action OnDamaged;
    public System.Action OnHealed;

    private void Awake()
    {
        CurrentHP = MaxHP;
    }

    public void ApplyDamage(int amount)
    {
        if (IsDead) return;

        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Debug.Log($"[Health] {EntityName} took {amount} dmg -> {CurrentHP}/{MaxHP} HP");

        OnDamaged?.Invoke();

        if (IsDead)
        {
            Debug.Log($"[Health] {EntityName} died.");
            OnDeath?.Invoke();
        }
    }

    public void SetMaxHP(int newMax)
    {
        MaxHP = newMax;
        CurrentHP = newMax;
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        Debug.Log($"[Health] {EntityName} healed {amount} -> {CurrentHP}/{MaxHP} HP");
        OnHealed?.Invoke(); // new
    }
}