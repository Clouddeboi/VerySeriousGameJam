using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public StatusEffectSystem StatusEffects;
    public FloatingTextSpawner TextSpawner;

    public void ResolveSingleHit(Attack attack, Health target)
    {
        target.ApplyDamage(attack.BaseDamage);
        TextSpawner.SpawnDamageNumber(attack.BaseDamage, target.transform.position + Vector3.up * 0.5f);
    }
}