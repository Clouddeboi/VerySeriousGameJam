public struct Attack
{
    public int BaseDamage;
    public int HitCount;
    public ElementSymbol Element;
    public WeaponSymbol Weapon;
    public bool IsCrit;
    public Health Source;
    public int SelfDamageAmount;
    public EffectDataSO SelfAppliedEffect;

    public override string ToString() =>
        $"Attack(dmg={BaseDamage}, hits={HitCount}, element={Element?.DisplayName ?? "None"}, crit={IsCrit})";
}