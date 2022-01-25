using UnityEngine;
using redd096;
using UnityEngine.InputSystem;

[AddComponentMenu("redd096/Tasks FSM/Action/Input/Reload By Input")]
public class ReloadByInput : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] WeaponComponent component;
    [SerializeField] PlayerInput playerInput;

    [Header("Reload")]
    [SerializeField] string inputName = "Reload";

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (component == null) component = GetStateMachineComponent<WeaponComponent>();
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

        //on input down
        if(playerInput.actions.FindAction(inputName).triggered)
        {
            if(component.EquippedWeapon)
            {
                //reload weapon if is a range weapon
                if (component.EquippedWeapon is WeaponRange weaponRange)
                {
                    weaponRange.Reload();
                }
            }
        }
    }
}
