using UnityEngine;

public class ExitFeedbacks : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] ExitInteractable interactable;

    [Header("Animator - default get in children")]
    [SerializeField] Animator anim = default;
    [SerializeField] string closeAnimation = "Close";
    [SerializeField] string openAnimation = "Open";

    void OnEnable()
    {
        //get references
        if (interactable == null) interactable = GetComponentInParent<ExitInteractable>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        //add events
        if(interactable)
        {
            interactable.onOpen += OnOpen;
            interactable.onClose += OnClose;
        }
    }

    void OnDisable()
    {
        //remove events
        if (interactable)
        {
            interactable.onOpen -= OnOpen;
            interactable.onClose -= OnClose;
        }
    }

    void OnOpen()
    {
        //move to open animation
        if(anim)
        {
            anim.Play(openAnimation);
        }
    }

    void OnClose()
    {
        //move to close animation
        if (anim)
        {
            anim.Play(closeAnimation);
        }
    }
}
