using UnityEngine;
using redd096;
using UnityEngine.InputSystem;

[AddComponentMenu("redd096/Tasks FSM/Action/Input/Attack By Input")]
public class AttackByInput : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] WeaponComponent component;
    [SerializeField] PlayerInput playerInput;

    [Header("Attack")]
    [SerializeField] string inputName = "Attack";

    bool isAttacking;
    bool inputValue;

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

        //get input
        inputValue = playerInput.actions.FindAction(inputName).phase == InputActionPhase.Started;

        //when press, attack
        if (inputValue && isAttacking == false)
        {
            isAttacking = true;

            if (component && component.CurrentWeapon)
                component.CurrentWeapon.PressAttack();
        }
        //on release, stop it if automatic shoot
        else if (inputValue == false && isAttacking)
        {
            isAttacking = false;

            if (component && component.CurrentWeapon)
                component.CurrentWeapon.ReleaseAttack();
        }
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //be sure to stop attack
        if(isAttacking)
        {
            isAttacking = false;

            if(component && component.CurrentWeapon)
                component.CurrentWeapon.ReleaseAttack();
        }
    }
}
