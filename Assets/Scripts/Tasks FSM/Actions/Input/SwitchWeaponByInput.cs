using UnityEngine;
using redd096;
using UnityEngine.InputSystem;

[AddComponentMenu("redd096/Tasks FSM/Action/Input/Switch Weapon By Input")]
public class SwitchWeaponByInput : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] WeaponComponent weaponComponent;
    [SerializeField] PlayerInput playerInput;

    [Header("Switch Weapon")]
    [SerializeField] string inputName = "Switch Weapon";
    [SerializeField] bool canSwitchWithMouseScroll = true;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (weaponComponent == null) weaponComponent = GetStateMachineComponent<WeaponComponent>();
        if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

        //show warnings if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (weaponComponent == null || playerInput == null || playerInput.actions == null)
            return;

        //on input down
        if (playerInput.actions.FindAction(inputName).triggered)
        {
            //switch weapon
            weaponComponent.SwitchWeapon();
        }
        //or when scroll with mouse
        else if (canSwitchWithMouseScroll && Mouse.current != null && Mouse.current.scroll.IsActuated())
        {
            //switch weapon
            weaponComponent.SwitchWeapon(Mouse.current.scroll.ReadValue().y > 0);
        }
    }
}
