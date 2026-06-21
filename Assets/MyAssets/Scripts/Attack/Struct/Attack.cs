public struct Attack
{
    public int BaseDamage;
    public int HitCount;
    public ElementSymbol Element;
    public bool IsCrit;
    public int FreezeDurationOverride;

    public override string ToString()
    {
        string e = Element ? Element.DisplayName : "None";
        return $"Attack(dmg={BaseDamage}, hits={HitCount}, element={e}, crit={IsCrit})";
    }
}