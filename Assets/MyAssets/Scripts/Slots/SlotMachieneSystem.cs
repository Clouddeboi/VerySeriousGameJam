using UnityEngine;

public class SlotMachineSystem : MonoBehaviour
{
    [Header("Default Reel Configs (used when no character override exists)")]
    public WeaponReelConfig DefaultWeaponConfig;
    public ElementReelConfig DefaultElementConfig;
    public ModifierReelConfig DefaultModifierConfig;

    private Reel<WeaponSymbol> weaponReel;
    private Reel<ElementSymbol> elementReel;
    private Reel<ModifierSymbol> modifierReel;

    private WeaponSymbol weaponLock;
    private Character currentCharacter;

    private CharacterRunState RunData => RunState.Current.GetCharacter(currentCharacter);

    // Character's own lock wins; otherwise a lock gained this run
    private WeaponSymbol EffectiveWeaponLock
        => weaponLock != null ? weaponLock : RunData?.LockedWeapon;

    private void Awake()
    {
        InitializeReels(null);
    }

    public void LoadCharacter(Character character)
    {
        currentCharacter = character;
        weaponLock = character?.WeaponLock;

        var weaponConfig = character?.WeaponReelConfig ?? DefaultWeaponConfig;
        var elementConfig = character?.ElementReelConfig ?? DefaultElementConfig;
        var modifierConfig = character?.ModifierReelConfig ?? DefaultModifierConfig;

        InitializeReels(weaponConfig, elementConfig, modifierConfig);
        ApplySavedWeights();
        ResetAllLocks();

        Debug.Log($"[SlotMachine] Loaded config for {character?.CharacterName ?? "default"}. " +
                  $"Weapon lock: {EffectiveWeaponLock?.DisplayName ?? "none"}");
    }

    private void ApplySavedWeights()
    {
        var data = RunData;
        if (data == null) return;
        weaponReel?.ApplyWeights(data.WeaponWeights);
        elementReel?.ApplyWeights(data.ElementWeights);
        modifierReel?.ApplyWeights(data.ModifierWeights);
    }

    private void InitializeReels(
        WeaponReelConfig weaponConfig = null,
        ElementReelConfig elementConfig = null,
        ModifierReelConfig modifierConfig = null)
    {
        weaponConfig ??= DefaultWeaponConfig;
        elementConfig ??= DefaultElementConfig;
        modifierConfig ??= DefaultModifierConfig;

        if (weaponConfig != null)
            weaponReel = new Reel<WeaponSymbol>(weaponConfig.Entries);
        if (elementConfig != null)
            elementReel = new Reel<ElementSymbol>(elementConfig.Entries);
        if (modifierConfig != null)
            modifierReel = new Reel<ModifierSymbol>(modifierConfig.Entries);
    }

    public SlotResult Spin()
    {
        WeaponSymbol weapon = EffectiveWeaponLock != null
            ? EffectiveWeaponLock
            : weaponReel?.Spin();

        var data = RunData;
        ElementSymbol element = data?.LockedElement != null
            ? data.LockedElement
            : elementReel?.Spin();
        ModifierSymbol modifier = data?.LockedModifier != null
            ? data.LockedModifier
            : modifierReel?.Spin();

        var result = new SlotResult
        {
            Weapon = weapon,
            Element = element,
            Modifier = modifier
        };

        Debug.Log($"[Slot] Spin result: {result}");
        return result;
    }

    public bool IsWeaponLocked => EffectiveWeaponLock != null;

    public void ToggleLock(string reelName)
    {
        var data = RunData;
        if (reelName == "Weapon" && EffectiveWeaponLock != null)
        {
            Debug.Log("[SlotMachine] Weapon reel is locked to character — cannot toggle.");
            return;
        }
        if (reelName == "Element" && data?.LockedElement != null) return;
        if (reelName == "Modifier" && data?.LockedModifier != null) return;

        switch (reelName)
        {
            case "Weapon": weaponReel.Locked = !weaponReel.Locked; break;
            case "Element": elementReel.Locked = !elementReel.Locked; break;
            case "Modifier": modifierReel.Locked = !modifierReel.Locked; break;
        }
    }

    public bool WeaponLocked => EffectiveWeaponLock != null || (weaponReel?.Locked ?? false);
    public bool ElementLocked => RunData?.LockedElement != null || (elementReel?.Locked ?? false);
    public bool ModifierLocked => RunData?.LockedModifier != null || (modifierReel?.Locked ?? false);

    // Weight changes are saved to the character's run state and rebuilt on the next LoadCharacter.
    // target defaults to the character currently loaded.
    public void AdjustWeaponWeight(WeaponSymbol symbol, float delta, Character target = null)
        => AdjustWeight(target, c => c?.WeaponReelConfig ?? DefaultWeaponConfig,
            cfg => cfg.Entries, s => s.WeaponWeights, (s, w) => s.WeaponWeights = w, symbol, delta);

    public void AdjustElementWeight(ElementSymbol symbol, float delta, Character target = null)
        => AdjustWeight(target, c => c?.ElementReelConfig ?? DefaultElementConfig,
            cfg => cfg.Entries, s => s.ElementWeights, (s, w) => s.ElementWeights = w, symbol, delta);

    public void AdjustModifierWeight(ModifierSymbol symbol, float delta, Character target = null)
        => AdjustWeight(target, c => c?.ModifierReelConfig ?? DefaultModifierConfig,
            cfg => cfg.Entries, s => s.ModifierWeights, (s, w) => s.ModifierWeights = w, symbol, delta);

    private void AdjustWeight<TConfig, TSymbol>(Character target,
        System.Func<Character, TConfig> getConfig,
        System.Func<TConfig, WeightedEntry<TSymbol>[]> getEntries,
        System.Func<CharacterRunState, float[]> getSaved,
        System.Action<CharacterRunState, float[]> setSaved,
        TSymbol symbol, float delta) where TConfig : Object
    {
        if (target == null) target = currentCharacter;
        var state = RunState.Current.GetCharacter(target);
        var config = getConfig(target);
        if (state == null || config == null) return;

        var reel = new Reel<TSymbol>(getEntries(config));
        reel.ApplyWeights(getSaved(state));
        reel.AdjustWeight(symbol, delta);
        setSaved(state, reel.SnapshotWeights());

        if (target == currentCharacter) ApplySavedWeights();
    }

    public void ResetAllLocks()
    {
        if (EffectiveWeaponLock == null && weaponReel != null) weaponReel.Locked = false;
        if (elementReel != null) elementReel.Locked = false;
        if (modifierReel != null) modifierReel.Locked = false;
    }

    // Drops all run weight upgrades and locks for every character
    public void ResetAllWeights()
    {
        RunState.Current.ResetReelUpgrades();
        if (currentCharacter != null) LoadCharacter(currentCharacter);
    }

    public WeaponSymbol LockedWeapon => EffectiveWeaponLock;
}