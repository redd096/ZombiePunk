using UnityEngine;
using redd096;

public class ComboComponentFeedback : FeedbackRedd096<ComboComponent>
{
    [Header("When get points from kill")]
    [SerializeField] FeedbackStructRedd096 feedbackOnAddPoint = default;

    [Header("Feedback when bar reach 100%")]
    [SerializeField] FeedbackStructRedd096 feedbackOnReachLimit = default;

    [Header("Feedback when finish time and bar start to decrease again")]
    [SerializeField] FeedbackStructRedd096 feedbackOnUnlockLimit = default;

    [Header("Feedback on Active")]
    [SerializeField] FeedbackStructRedd096 feedbackOnActive = default;

    [Header("Feedback on Deactive")]
    [SerializeField] FeedbackStructRedd096 feedbackOnDeactive = default;

    protected override void AddEvents()
    {
        owner.onAddPoint += OnAddPoint;
        owner.onReachLimit += OnReachLimit;
        owner.onUnlockLimit += OnUnlockLimit;
        owner.onActive += OnActive;
        owner.onDeactive += OnDeactive;
    }

    protected override void RemoveEvents()
    {
        owner.onAddPoint -= OnAddPoint;
        owner.onReachLimit -= OnReachLimit;
        owner.onUnlockLimit -= OnUnlockLimit;
        owner.onActive -= OnActive;
        owner.onDeactive -= OnDeactive;
    }

    void Update()
    {
        //update combo sliders
        if (GameManager.instance && GameManager.instance.uiManager)
        {
            GameManager.instance.uiManager.UpdateComboSlider(owner.CurrentCombo / owner.ComboToReach);
            GameManager.instance.uiManager.UpdateSuperWeaponSlider(owner.TimerWeapon / owner.DurationWeapon);
        }
    }

    private void OnAddPoint()
    {
        //instantiate feedback
        InstantiateFeedback(feedbackOnAddPoint);
    }

    private void OnReachLimit()
    {
        //instantiate feedback
        InstantiateFeedback(feedbackOnReachLimit);

        //set combo full when reach limit
        if (GameManager.instance && GameManager.instance.uiManager)
            GameManager.instance.uiManager.SetComboIsFull(true);
    }

    private void OnUnlockLimit()
    {
        //instantiate feedback
        InstantiateFeedback(feedbackOnUnlockLimit);

        //set combo NOT full, when come back to charge state
        if (GameManager.instance && GameManager.instance.uiManager)
            GameManager.instance.uiManager.SetComboIsFull(false);
    }

    private void OnActive()
    {
        //instantiate feedback
        InstantiateFeedback(feedbackOnActive);

        //show super weapon bar if necessary
        if (GameManager.instance && GameManager.instance.uiManager)
            GameManager.instance.uiManager.SetSuperWeaponIsActive(true);

        //set combo NOT full, when active super weapon
        if (GameManager.instance && GameManager.instance.uiManager)
            GameManager.instance.uiManager.SetComboIsFull(false);
    }

    private void OnDeactive()
    {
        //instantiate feedback
        InstantiateFeedback(feedbackOnDeactive);

        //hide super weapon bar if necessary
        if (GameManager.instance && GameManager.instance.uiManager)
            GameManager.instance.uiManager.SetSuperWeaponIsActive(false);
    }
}
