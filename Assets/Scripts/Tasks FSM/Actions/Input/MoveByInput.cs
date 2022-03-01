using UnityEngine;
using redd096;
using UnityEngine.InputSystem;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Input/Move By Input")]
public class MoveByInput : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] MovementComponent component;
    [SerializeField] PlayerInput playerInput;

    [Header("Movement")]
    [SerializeField] string inputName = "Move";
    [SerializeField] float speed = 5;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if(component == null) component = GetStateMachineComponent<MovementComponent>();
        if(playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

        //show warnings if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (component == null || playerInput == null || playerInput.actions == null)
            return;

        //move in direction
        component.MoveInDirection(playerInput.actions.FindAction(inputName).ReadValue<Vector2>(), speed);
    }
}
