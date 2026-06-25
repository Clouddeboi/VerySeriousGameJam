using UnityEngine;

public class SlotMachineSystem : MonoBehaviour
{
    [Header("Reel Configs")]
    public WeaponReelConfig WeaponConfig;
    public ElementReelConfig ElementConfig;
    public ModifierReelConfig ModifierConfig;

    public bool WeaponLocked => weaponReel.Locked;
    public bool ElementLocked => elementReel.Locked;
    public bool ModifierLocked => modifierReel.Locked;

    private Reel<WeaponSymbol> weaponReel;
    private Reel<ElementSymbol> elementReel;
    private Reel<ModifierSymbol> modifierReel;

    private void Awake()
    {
        weaponReel = new Reel<WeaponSymbol>(WeaponConfig.Entries);
        elementReel = new Reel<ElementSymbol>(ElementConfig.Entries);
        modifierReel = new Reel<ModifierSymbol>(ModifierConfig.Entries);
    }

    public SlotResult Spin()
    {
        var result = new SlotResult
        {
            Weapon = weaponReel.Spin(),
            Element = elementReel.Spin(),
            Modifier = modifierReel.Spin()
        };

        Debug.Log($"[Slot] Spin result: {result}");
        return result;
    }

    public void ToggleLock(string reelName)
    {
        switch (reelName)
        {
            case "Weapon": weaponReel.Locked = !weaponReel.Locked; break;
            case "Element": elementReel.Locked = !elementReel.Locked; break;
            case "Modifier": modifierReel.Locked = !modifierReel.Locked; break;
        }
    }

    public void ResetAllLocks()
    {
        weaponReel.Locked = false;
        elementReel.Locked = false;
        modifierReel.Locked = false;
    }

    public void ResetAllWeights()
    {
        weaponReel.ResetToOriginalWeights();
        elementReel.ResetToOriginalWeights();
        modifierReel.ResetToOriginalWeights();
    }

    public void AdjustElementWeight(ElementSymbol symbol, float delta) => elementReel.AdjustWeight(symbol, delta);
    public void AdjustWeaponWeight(WeaponSymbol symbol, float delta) => weaponReel.AdjustWeight(symbol, delta);

}