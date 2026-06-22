using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public StatusEffectSystem StatusEffects;

    public void ResolveSingleHit(Attack attack, Health target)
    {
        target.ApplyDamage(attack.BaseDamage);
    }
}