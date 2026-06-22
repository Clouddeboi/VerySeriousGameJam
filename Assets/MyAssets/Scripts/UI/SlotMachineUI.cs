using UnityEngine;
using TMPro;

public class SlotMachineUI : MonoBehaviour
{
    public SlotMachineSystem SlotMachine;

    public TMP_Text WeaponText;
    public TMP_Text ElementText;
    public TMP_Text ModifierText;

    public GameObject WeaponLockIcon;
    public GameObject ElementLockIcon;
    public GameObject ModifierLockIcon;

    public void RefreshDisplay(SlotResult result)
    {
        WeaponText.text = result.Weapon ? result.Weapon.DisplayName : "-";
        ElementText.text = result.Element ? result.Element.DisplayName : "-";
        ModifierText.text = result.Modifier ? result.Modifier.DisplayName : "-";

        RefreshLockIcons();
    }

    public void RefreshLockIcons()
    {
        WeaponLockIcon.SetActive(SlotMachine.WeaponLocked);
        ElementLockIcon.SetActive(SlotMachine.ElementLocked);
        ModifierLockIcon.SetActive(SlotMachine.ModifierLocked);
    }

    public void OnWeaponLockClicked()
    {
        SlotMachine.ToggleLock("Weapon");
        RefreshLockIcons(); //Immediate feedback, don't wait for next spin
    }

    public void OnElementLockClicked()
    {
        SlotMachine.ToggleLock("Element");
        RefreshLockIcons();
    }

    public void OnModifierLockClicked()
    {
        SlotMachine.ToggleLock("Modifier");
        RefreshLockIcons();
    }
}