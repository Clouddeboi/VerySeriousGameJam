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

    public FloatingTextSpawner TextSpawner;

    [Header("Damage Number Offset")]
    public Vector3 DamageNumberOffset = new Vector3(0f, 0.5f, 0f);
    public Vector3 HealNumberOffset = new Vector3(0f, 0.5f, 0f);

    private void Awake()
    {
        CurrentHP = MaxHP;
    }

    public void SetMaxHP(int newMax)
    {
        MaxHP = newMax;
        CurrentHP = newMax;
    }

    public void ApplyDamage(int amount)
    {
        if (IsDead) return;

        int actualDamage = Mathf.Min(amount, CurrentHP);
        CurrentHP -= actualDamage;

        Debug.Log($"[Health] {EntityName} took {actualDamage} dmg -> {CurrentHP}/{MaxHP} HP");

        OnDamaged?.Invoke();

        if (actualDamage > 0 && TextSpawner != null)
            TextSpawner.SpawnDamageNumber(actualDamage, transform.position + DamageNumberOffset);

        if (IsDead)
        {
            Debug.Log($"[Health] {EntityName} died.");
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        int actualHeal = Mathf.Min(amount, MaxHP - CurrentHP);
        CurrentHP += actualHeal;

        Debug.Log($"[Health] {EntityName} healed {actualHeal} -> {CurrentHP}/{MaxHP} HP");

        OnHealed?.Invoke();

        if (actualHeal > 0 && TextSpawner != null)
            TextSpawner.SpawnHealNumber(actualHeal, transform.position + HealNumberOffset);
    }
}