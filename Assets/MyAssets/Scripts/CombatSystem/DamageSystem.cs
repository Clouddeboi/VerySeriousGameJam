using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public BuffDebuffSystem BuffDebuff;

    public void ResolveDamage(Health source, Health target, int baseAmount, bool isEnemyAttack,
        ElementType element = ElementType.None, EnemyData targetData = null)
    {
        if (BuffDebuff.IsIncomingDamageBlocked(target, isEnemyAttack))
        {
            Debug.Log($"[Damage] {target.EntityName}'s Shield blocked the attack.");
            return;
        }

        float mult = source != null ? BuffDebuff.GetOutgoingDamageMultiplier(source) : 1f;
        int damage = Mathf.RoundToInt(baseAmount * mult);

        // Apply elemental affinity if target has EnemyDataSO and element is provided
        if (element != ElementType.None && targetData != null)
        {
            var response = targetData.GetElementalResponse(element);
            damage = ApplyAffinity(damage, response, element, target.EntityName);
        }

        target.ApplyDamage(damage);
    }

    private int ApplyAffinity(int damage, ElementalResponse response,
        ElementType element, string targetName)
    {
        switch (response)
        {
            case ElementalResponse.Weak:
                int weakDamage = Mathf.RoundToInt(damage * 1.5f);
                Debug.Log($"[Damage] {targetName} is weak to {element}: {damage} -> {weakDamage}");
                return weakDamage;
            case ElementalResponse.Resistant:
                int resDamage = Mathf.RoundToInt(damage * 0.5f);
                Debug.Log($"[Damage] {targetName} resists {element}: {damage} -> {resDamage}");
                return resDamage;
            case ElementalResponse.Immune:
                Debug.Log($"[Damage] {targetName} is immune to {element}: 0 damage");
                return 0;
            default:
                return damage;
        }
    }

    public void ResolveSingleHit(Attack attack, Health target, EnemyData targetData = null)
    {
        ElementType element = attack.Element != null ? attack.Element.Type : ElementType.None;
        ResolveDamage(attack.Source, target, attack.BaseDamage,
            isEnemyAttack: false, element, targetData);
    }
}