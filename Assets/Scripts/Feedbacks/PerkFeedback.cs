using UnityEngine;

[AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Perks Feedback")]
public class PerkFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] PerksComponent perksComponent = default;

    [Header("Parent to use when instantiate feedback - default is perksComponent transform")]
    [SerializeField] Transform feedbackParent = default;

    [Header("Update UI")]
    [SerializeField] bool updatePerkImage = true;

    void OnEnable()
    {
        //get references
        if (perksComponent == null) perksComponent = GetComponentInParent<PerksComponent>();
        if (feedbackParent == null && perksComponent != null) feedbackParent = perksComponent.transform;

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

            //call UIManager
            if (updatePerkImage && GameManager.instance && GameManager.instance.uiManager)
                GameManager.instance.uiManager.SetPerkUsed(perksComponent.EquippedPerk);
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
}
