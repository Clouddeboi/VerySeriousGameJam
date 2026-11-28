using System.Collections.Generic;

public enum ReelKind { Weapon, Element, Modifier }

// Everything that changes during a run lives here, never in ScriptableObjects.
// Shop items, sacrifices and blessings should write to this.
public class RunState
{
    public static RunState Current { get; private set; } = new RunState();

    public int Floor;
    public int RoomsCleared;

    private readonly Dictionary<Character, CharacterRunState> characters =
        new Dictionary<Character, CharacterRunState>();

    // Discards all run data; call once when a new run begins
    public static void StartNew() => Current = new RunState();

    public CharacterRunState GetCharacter(Character character)
    {
        if (character == null) return null;
        if (!characters.TryGetValue(character, out var state))
        {
            state = new CharacterRunState();
            characters[character] = state;
        }
        return state;
    }

    public void ResetReelUpgrades()
    {
        foreach (var c in characters.Values) c.ResetReelUpgrades();
    }
}

public class CharacterRunState
{
    // Saved reel weights, parallel to the character's reel config entries (null = config defaults)
    public float[] WeaponWeights;
    public float[] ElementWeights;
    public float[] ModifierWeights;

    // Locks gained this run. The character's own WeaponLock is not stored here and can't be removed.
    public WeaponSymbol LockedWeapon;
    public ElementSymbol LockedElement;
    public ModifierSymbol LockedModifier;

    public int MaxHPBonus;
    public float DamageBonusPercent;

    public void RemoveLock(ReelKind reel)
    {
        switch (reel)
        {
            case ReelKind.Weapon: LockedWeapon = null; break;
            case ReelKind.Element: LockedElement = null; break;
            case ReelKind.Modifier: LockedModifier = null; break;
        }
    }

    public void RemoveAllLocks()
    {
        LockedWeapon = null;
        LockedElement = null;
        LockedModifier = null;
    }

    public void ResetReelUpgrades()
    {
        WeaponWeights = null;
        ElementWeights = null;
        ModifierWeights = null;
        RemoveAllLocks();
    }
}
