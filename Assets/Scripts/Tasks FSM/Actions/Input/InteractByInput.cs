using UnityEngine;
using redd096;
using UnityEngine.InputSystem;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Input/Interact By Input")]
public class InteractByInput : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] InteractComponent component;
    [SerializeField] PlayerInput playerInput;

    [Header("Interact")]
    [SerializeField] string inputName = "Interact";

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (component == null) component = GetStateMachineComponent<InteractComponent>();
        if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

        //show warnings if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (component == null || playerInput == null || playerInput.actions == null)
            return;

        //when press input, interact
        if (playerInput.actions.FindAction(inputName).triggered)
            component.Interact();
    }
}
