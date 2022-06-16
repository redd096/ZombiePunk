using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Perks Feedback")]
public class PerkFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent and children")]
    [SerializeField] PerksComponent perksComponent = default;
    [SerializeField] Animator anim = default;

    [Header("Parent to use when instantiate feedback - default is perksComponent transform")]
    [SerializeField] Transform feedbackParent = default;

    [Header("Update UI")]
    [SerializeField] bool updatePerkImage = true;

    [Header("Override sprite rotation - default get from self and parent")]
    [SerializeField] FlipSpriteFeedback flipSpriteFeedback = default;
    [SerializeField] AimComponent aimComponent = default;
    [SerializeField] MovementComponent movementComponent = default;

    Coroutine overrideSpriteRotationCoroutine;

    void OnEnable()
    {
        //get references
        if (perksComponent == null) perksComponent = GetComponentInParent<PerksComponent>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (feedbackParent == null && perksComponent != null) feedbackParent = perksComponent.transform;
        if (flipSpriteFeedback == null) flipSpriteFeedback = GetComponent<FlipSpriteFeedback>();
        if (aimComponent == null) aimComponent = GetComponentInParent<AimComponent>();
        if (movementComponent == null) movementComponent = GetComponentInParent<MovementComponent>();

        //set default
        OnEquipPerk(perksComponent ? perksComponent.EquippedPerk : null);

        //add events
        if (perksComponent)
        {
            perksComponent.onUsePerk += OnUsePerk;
            perksComponent.onEquipPerk += OnEquipPerk;
            perksComponent.onUnequipPerk += OnUnequipPerk;
        }
    }

    void OnDisable()
    {
        //remove events
        if (perksComponent)
        {
            perksComponent.onUsePerk -= OnUsePerk;
            perksComponent.onEquipPerk -= OnEquipPerk;
            perksComponent.onUnequipPerk -= OnUnequipPerk;
        }
    }

    void OnUsePerk(bool isSuccessfull)
    {
        //if use a perk
        if (isSuccessfull && perksComponent.EquippedPerk)
        {
            //instantiate feedback
            perksComponent.EquippedPerk.Feedback.InstantiateFeedback(feedbackParent);
            perksComponent.EquippedPerk.CameraShake.TryShake();
            perksComponent.EquippedPerk.GamepadVibration.TryVibration();

            //do animation (if setted)
            if (anim && perksComponent.EquippedPerk && perksComponent.EquippedPerk.UseAnimation)
            {
                anim.SetTrigger(perksComponent.EquippedPerk.AnimatorTrigger);
            }

            //call UIManager
            if (updatePerkImage && GameManager.instance && GameManager.instance.uiManager)
                GameManager.instance.uiManager.SetPerkUsed(perksComponent.EquippedPerk);

            //override sprite rotation (start coroutine)
            if (perksComponent.EquippedPerk.OverrideRotationSprite)
            {
                if (overrideSpriteRotationCoroutine != null)
                    StopCoroutine(overrideSpriteRotationCoroutine);

                overrideSpriteRotationCoroutine = StartCoroutine(OverrideSpriteRotationCoroutine(perksComponent.EquippedPerk));
            }
        }
    }

    void OnEquipPerk(PerkData perk)
    {
        //set UIManager
        if (updatePerkImage && GameManager.instance && GameManager.instance.uiManager)
            GameManager.instance.uiManager.SetPerkImage(perk);
    }

    void OnUnequipPerk(PerkData perk)
    {
        //set UIManager
        if (updatePerkImage && GameManager.instance && GameManager.instance.uiManager)
            GameManager.instance.uiManager.SetPerkImage(null);
    }

    IEnumerator OverrideSpriteRotationCoroutine(PerkData perk)
    {
        //deactive flip sprite
        if (flipSpriteFeedback)
            flipSpriteFeedback.enabled = false;

        //set rotation based on perk rules
        float time = Time.time + perk.DurationOverride;
        while (time > Time.time)
        {
            if (perk == null || flipSpriteFeedback == null)
                break;

            //rotate to aim or movement direction
            bool isLookingRight = perk.RotateToAimDirection && aimComponent ? aimComponent.IsLookingRight : (movementComponent ? movementComponent.IsMovingRight : true);
            flipSpriteFeedback.OnChangeAimDirection(isLookingRight);

            yield return null;
        }

        //re-enable flip sprite
        if (flipSpriteFeedback)
            flipSpriteFeedback.enabled = true;
    }
}
