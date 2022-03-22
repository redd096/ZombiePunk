using UnityEngine;
using redd096;
using UnityEngine.InputSystem;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Generic/Lobby Menu Action")]
public class LobbyMenuAction : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] InteractComponent interactComponent;
    [SerializeField] PlayerInput playerInput;

    [Header("Resume Button")]
    [SerializeField] string buttonName = "Resume";

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (interactComponent == null) interactComponent = GetStateMachineComponent<InteractComponent>();
        if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();
 
        //show warning if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (interactComponent == null || playerInput == null || playerInput.actions == null)
            return;

        //if press input, call event
        if (playerInput.actions.FindAction(buttonName).triggered)
        {
            interactComponent.inputEventForStateMachines?.Invoke();
        }
    }
}
