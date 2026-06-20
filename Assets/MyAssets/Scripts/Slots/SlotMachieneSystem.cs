using UnityEngine;

public class SlotMachineSystem : MonoBehaviour
{
    [Header("Reel Configs")]
    public WeaponReelConfig WeaponConfig;
    public ElementReelConfig ElementConfig;
    public ModifierReelConfig ModifierConfig;

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
}