using UnityEngine;

public class SlotMachineUI : MonoBehaviour
{
    public SlotMachineSystem SlotMachine;

    public SlotReelVisual WeaponReelVisual;
    public SlotReelVisual ElementReelVisual;
    public SlotReelVisual ModifierReelVisual;

    public GameObject WeaponLockIcon;
    public GameObject ElementLockIcon;
    public GameObject ModifierLockIcon;

    [Header("Idle State")]
    public Sprite WeaponIdleSprite;
    public Sprite ElementIdleSprite;
    public Sprite ModifierIdleSprite;

    private int reelsFinished;
    private System.Action onAllReelsFinished;

    public void PlaySpinAnimation(SlotResult result, System.Action onComplete)
    {
        reelsFinished = 0;
        onAllReelsFinished = onComplete;

        Sprite weaponSprite = result.Weapon ? result.Weapon.Icon : null;
        Sprite elementSprite = result.Element ? result.Element.Icon : null;
        Sprite modifierSprite = result.Modifier ? result.Modifier.Icon : null;

        WeaponReelVisual.PlaySpin(weaponSprite, OnReelFinished);
        ElementReelVisual.PlaySpin(elementSprite, OnReelFinished);
        ModifierReelVisual.PlaySpin(modifierSprite, OnReelFinished);

        RefreshLockIcons();
    }

    //Snaps all three reels back to a neutral idle look, no animation, instant
    public void ResetReelsToIdle()
    {
        WeaponReelVisual.SetIdle(WeaponIdleSprite);
        ElementReelVisual.SetIdle(ElementIdleSprite);
        ModifierReelVisual.SetIdle(ModifierIdleSprite);
    }

    private void OnReelFinished()
    {
        reelsFinished++;
        if (reelsFinished >= 3)
            onAllReelsFinished?.Invoke();
    }

    public void RefreshLockIcons()
    {
        WeaponLockIcon.SetActive(SlotMachine.WeaponLocked);
        ElementLockIcon.SetActive(SlotMachine.ElementLocked);
        ModifierLockIcon.SetActive(SlotMachine.ModifierLocked);
    }

    public void OnWeaponLockClicked() { SlotMachine.ToggleLock("Weapon"); RefreshLockIcons(); }
    public void OnElementLockClicked() { SlotMachine.ToggleLock("Element"); RefreshLockIcons(); }
    public void OnModifierLockClicked() { SlotMachine.ToggleLock("Modifier"); RefreshLockIcons(); }
}