using UnityEngine;

public class SlotMachineUI : MonoBehaviour
{
    public SlotMachineSystem SlotMachine;
    public CameraShake CameraShake;

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

    [Header("Camera Shake On Landing")]
    public float CameraShakeDuration = 0.15f;
    public float CameraShakeMagnitude = 0.1f;

    public void PlaySpinAnimation(SlotResult result, System.Action onComplete)
    {
        RefreshLockIcons();

        Sprite weaponSprite = result.Weapon ? result.Weapon.Icon : null;
        Sprite elementSprite = result.Element ? result.Element.Icon : null;
        Sprite modifierSprite = result.Modifier ? result.Modifier.Icon : null;

        //Sequential order, weapon lands -> element starts -> element lands -> modifier starts -> modifier lands -> done
        WeaponReelVisual.PlaySpin(weaponSprite, () =>
        {
            CameraShake.Shake(CameraShakeDuration, CameraShakeMagnitude);

            ElementReelVisual.PlaySpin(elementSprite, () =>
            {
                CameraShake.Shake(CameraShakeDuration, CameraShakeMagnitude);

                ModifierReelVisual.PlaySpin(modifierSprite, () =>
                {
                    CameraShake.Shake(CameraShakeDuration, CameraShakeMagnitude);
                    onComplete?.Invoke();
                });
            });
        });
    }

    public void ResetReelsToIdle()
    {
        WeaponReelVisual.SetIdle(WeaponIdleSprite);
        ElementReelVisual.SetIdle(ElementIdleSprite);
        ModifierReelVisual.SetIdle(ModifierIdleSprite);
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