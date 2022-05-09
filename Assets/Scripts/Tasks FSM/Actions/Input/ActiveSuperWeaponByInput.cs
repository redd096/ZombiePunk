using UnityEngine;
using redd096;
using UnityEngine.InputSystem;

[AddComponentMenu("redd096/Tasks FSM/Action/Input/Active Super Weapon By Input")]
public class ActiveSuperWeaponByInput : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] ComboComponent comboComponent = default;
    [SerializeField] PlayerInput playerInput = default;

    [Header("Use Perk")]
    [SerializeField] string inputName = "Active Super Weapon";
    [SerializeField] string secondInputName = "Second Active Super Weapon";

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (comboComponent == null) comboComponent = GetStateMachineComponent<ComboComponent>();
        if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

        //show warnings if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (comboComponent == null || playerInput == null || playerInput.actions == null)
            return;

        //when press input, active super weapon
        if ( (playerInput.actions.FindAction(inputName).triggered && CheckDoubleInput(false))               //check press first input and second is already pressed
            || (playerInput.actions.FindAction(secondInputName).triggered && CheckDoubleInput(true)) )      //check press second input and first is already pressed
        {
            comboComponent.ActiveCombo();
        }
    }

    bool CheckDoubleInput(bool checkFirstInput)
    {
        string inputToCheck = checkFirstInput ? inputName : secondInputName;
        InputAction inputAction = playerInput.actions.FindAction(inputToCheck);

        //if there are other inputs for this device
        if (inputAction.controls.Count > 0)
        {
            //check if also this input is pressed
            return inputAction.phase == InputActionPhase.Started;
        }

        //if there aren't other inputs for this device, return true to active combo just with one input
        return true;
    }
}
