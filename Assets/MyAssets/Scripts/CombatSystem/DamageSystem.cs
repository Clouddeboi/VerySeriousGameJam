using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public BuffDebuffSystem BuffDebuff;

    public void ResolveDamage(Health source, Health target, int baseAmount, bool isEnemyAttack,
        ElementType element = ElementType.None, EnemyData targetData = null,
        Character targetCharacter = null)
    {
        float mult = source != null ? BuffDebuff.GetOutgoingDamageMultiplier(source) : 1f;
        int damage = Mathf.RoundToInt(baseAmount * mult);

        // Elemental affinity comes from the enemy data or the character, whichever is the target
        if (element != ElementType.None && (targetData != null || targetCharacter != null))
        {
            var response = targetData != null
                ? targetData.GetElementalResponse(element)
                : targetCharacter.GetElementalResponse(element);
            damage = ApplyAffinity(damage, response, element, target.EntityName);

            if (response == ElementalResponse.Immune)
            {
                ShowImmune(target);
                return;
            }
        }

        // Defense Up and similar effects on the target
        damage = Mathf.RoundToInt(damage * BuffDebuff.GetIncomingDamageMultiplier(target));

        // Reflection is independent of blocking: it uses the damage that would have landed
        float reflect = BuffDebuff.GetReflectPercent(target);
        if (reflect > 0f && source != null && source != target && damage > 0)
        {
            int reflected = Mathf.RoundToInt(damage * reflect);
            if (reflected > 0)
            {
                Debug.Log($"[Damage] {target.EntityName} reflects {reflected} damage to {source.EntityName}");
                source.ApplyDamage(reflected);
            }
        }

        if (BuffDebuff.IsIncomingDamageBlocked(target, isEnemyAttack))
        {
            Debug.Log($"[Damage] {target.EntityName}'s Shield blocked the attack.");
            return;
        }

        target.ApplyDamage(damage);
    }

    private void ShowImmune(Health target)
    {
        if (target.TextSpawner == null) return;
        target.TextSpawner.SpawnDamageNumber(0, target.transform.position + target.DamageNumberOffset);
        target.TextSpawner.SpawnEffectAppliedText("Immune!", true,
            target.transform.position + target.DamageNumberOffset + Vector3.up * 0.6f);
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