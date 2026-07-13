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

    private void Awake()
    {
        InitializeReels(null);
    }

    public void LoadCharacter(Character character)
    {
        weaponLock = character?.WeaponLock;

        var weaponConfig = character?.WeaponReelConfig ?? DefaultWeaponConfig;
        var elementConfig = character?.ElementReelConfig ?? DefaultElementConfig;
        var modifierConfig = character?.ModifierReelConfig ?? DefaultModifierConfig;

        InitializeReels(weaponConfig, elementConfig, modifierConfig);
        ResetAllLocks();

        Debug.Log($"[SlotMachine] Loaded config for {character?.CharacterName ?? "default"}. " +
                  $"Weapon lock: {weaponLock?.DisplayName ?? "none"}");
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
        WeaponSymbol weapon = weaponLock != null
            ? weaponLock
            : weaponReel?.Spin();

        var result = new SlotResult
        {
            Weapon = weapon,
            Element = elementReel?.Spin(),
            Modifier = modifierReel?.Spin()
        };

        Debug.Log($"[Slot] Spin result: {result}");
        return result;
    }

    public bool IsWeaponLocked => weaponLock != null;

    public void ToggleLock(string reelName)
    {
        if (reelName == "Weapon" && weaponLock != null)
        {
            Debug.Log("[SlotMachine] Weapon reel is locked to character — cannot toggle.");
            return;
        }
        switch (reelName)
        {
            case "Weapon": weaponReel.Locked = !weaponReel.Locked; break;
            case "Element": elementReel.Locked = !elementReel.Locked; break;
            case "Modifier": modifierReel.Locked = !modifierReel.Locked; break;
        }
    }

    public bool WeaponLocked => weaponLock != null || (weaponReel?.Locked ?? false);
    public bool ElementLocked => elementReel?.Locked ?? false;
    public bool ModifierLocked => modifierReel?.Locked ?? false;

    public void AdjustWeaponWeight(WeaponSymbol symbol, float delta)
        => weaponReel?.AdjustWeight(symbol, delta);

    public void AdjustElementWeight(ElementSymbol symbol, float delta)
        => elementReel?.AdjustWeight(symbol, delta);

    public void ResetAllLocks()
    {
        if (weaponLock == null) weaponReel.Locked = false;
        elementReel.Locked = false;
        modifierReel.Locked = false;
    }

    public void ResetAllWeights()
    {
        weaponReel?.ResetToOriginalWeights();
        elementReel?.ResetToOriginalWeights();
        modifierReel?.ResetToOriginalWeights();
    }

    public WeaponSymbol LockedWeapon => weaponLock;
}