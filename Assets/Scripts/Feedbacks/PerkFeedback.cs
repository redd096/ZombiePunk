using UnityEngine;

[AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Perks Feedback")]
public class PerkFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] PerksComponent perksComponent = default;

    [Header("Parent to use - default is perksComponent transform")]
    [SerializeField] Transform feedbackParent = default;

    void OnEnable()
    {
        //get references
        if (perksComponent == null) perksComponent = GetComponentInParent<PerksComponent>();
        if (feedbackParent == null && perksComponent != null) feedbackParent = perksComponent.transform;

        //add events
        if (perksComponent)
        {
            perksComponent.onUsePerk += OnUsePerk;
        }
    }

    void OnDisable()
    {
        //remove events
        if (perksComponent)
        {
            perksComponent.onUsePerk -= OnUsePerk;
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
        }
    }
}
