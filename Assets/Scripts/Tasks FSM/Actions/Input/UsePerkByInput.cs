using UnityEngine;
using redd096;
using UnityEngine.InputSystem;

[AddComponentMenu("redd096/Tasks FSM/Action/Input/Use Perk By Input")]
public class UsePerkByInput : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] PerksComponent perksComponent = default;
    [SerializeField] PlayerInput playerInput = default;

    [Header("Use Perk")]
    [SerializeField] string inputName = "Use Perk";

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (perksComponent == null) perksComponent = GetStateMachineComponent<PerksComponent>();
        if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

        //show warnings if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (perksComponent == null || playerInput == null || playerInput.actions == null)
            return;

        //when press input, interact
        if (playerInput.actions.FindAction(inputName).triggered)
            perksComponent.UsePerk();
    }
}
