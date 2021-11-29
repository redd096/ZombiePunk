using UnityEngine;
using redd096;
using UnityEngine.InputSystem;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Input Press")]
public class CheckInputPress : ConditionTask
{
    enum EPressType { Pressed, Down }

    [Header("Necessary Components - default get in parent")]
    [SerializeField] PlayerInput playerInput;

    [Header("Button")]
    [SerializeField] EPressType pressType = EPressType.Down;
    [SerializeField] string buttonName = "Pause";

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

        //show warning if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
    }

    public override bool OnCheckTask()
    {
        if (playerInput == null)
            return false;

        //check if pressed or pressed down
        if (pressType == EPressType.Pressed)
            return playerInput.actions.FindAction(buttonName).phase == InputActionPhase.Started;
        else if (pressType == EPressType.Down)
            return playerInput.actions.FindAction(buttonName).triggered;

        return false;
    }
}
