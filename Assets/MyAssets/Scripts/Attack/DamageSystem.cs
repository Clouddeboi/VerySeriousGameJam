using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public void Resolve(Attack attack, Health target)
    {
        for (int i = 0; i < attack.HitCount; i++)
        {
            target.ApplyDamage(attack.BaseDamage);
            if (target.IsDead) break;
        }

        if (attack.Element != null && attack.Element.Type != ElementType.None)
            Debug.Log($"[DamageSystem] (TODO) Apply {attack.Element.Type} status effect to {target.EntityName}");
    }
}