public struct Attack
{
    public int BaseDamage;
    public int HitCount;
    public ElementSymbol Element;
    public WeaponSymbol Weapon;
    public bool IsCrit;
    public int FreezeDurationOverride;
    
    public bool IsOneShot;
    public int HealPlayerAmount;
    public int ConfusionDamageToPlayer;

    public override string ToString()
    {
        string e = Element ? Element.DisplayName : "None";
        return $"Attack(dmg={BaseDamage}, hits={HitCount}, element={e}, crit={IsCrit})";
    }
}