public struct Attack
{
    public int BaseDamage;
    public int HitCount;
    public ElementSymbol Element;
    public WeaponSymbol Weapon;
    public bool IsCrit;
    public int FreezeDurationOverride;
    public bool IsOneShot;
    public int ConfusionDamageToPlayer;
    public Health Source;
    public EffectDataSO AppliedPlayerBuff;
}