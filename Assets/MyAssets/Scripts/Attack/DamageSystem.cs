using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public void ResolveSingleHit(Attack attack, Health target)
    {
        target.ApplyDamage(attack.BaseDamage);

        if (attack.Element != null && attack.Element.Type != ElementType.None)
            Debug.Log($"[DamageSystem] (TODO) Apply {attack.Element.Type} status effect to {target.EntityName}");
    }
}