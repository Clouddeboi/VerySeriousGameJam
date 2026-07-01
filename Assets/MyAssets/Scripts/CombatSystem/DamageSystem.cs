using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public BuffDebuffSystem BuffDebuff;

    //Source = attacker (null if no buffs apply), isEnemyAttack = true when an enemy is attacking the player
    public void ResolveDamage(Health source, Health target, int baseAmount, bool isEnemyAttack)
    {
        if (BuffDebuff.IsIncomingDamageBlocked(target, isEnemyAttack))
        {
            Debug.Log($"[Damage] {target.EntityName}'s Shield blocked the attack.");
            return;
        }

        float mult = source != null ? BuffDebuff.GetOutgoingDamageMultiplier(source) : 1f;
        target.ApplyDamage(Mathf.RoundToInt(baseAmount * mult));
    }

    public void ResolveSingleHit(Attack attack, Health target) =>
        ResolveDamage(attack.Source, target, attack.BaseDamage, isEnemyAttack: false);
}