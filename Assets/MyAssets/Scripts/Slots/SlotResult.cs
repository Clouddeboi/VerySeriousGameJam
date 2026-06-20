public struct SlotResult
{
    public WeaponSymbol Weapon;
    public ElementSymbol Element;
    public ModifierSymbol Modifier;

    public override string ToString()
    {
        string w = Weapon ? Weapon.DisplayName : "None";
        string e = Element ? Element.DisplayName : "None";
        string m = Modifier ? Modifier.DisplayName : "None";
        return $"{w} | {e} | {m}";
    }
}