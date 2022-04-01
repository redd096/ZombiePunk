using UnityEngine;
using redd096;
using UnityEngine.InputSystem;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Input/Aim By Input")]
public class AimByInput : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] AimComponent component;
    [SerializeField] PlayerInput playerInput;

    [Header("For Mouse - default cam is MainCamera")]
    [SerializeField] Camera cam = default;
    [SerializeField] string mouseSchemeName = "KeyboardAndMouse";

    [Header("Aim")]
    [SerializeField] string inputName = "Aim";
    [SerializeField] bool resetWhenReleaseAnalogInput = false;

    Vector2 inputValue;
    Vector2 lastAnalogSavedValue = Vector2.right;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (component == null) component = GetStateMachineComponent<AimComponent>();
        if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();
        if (cam == null) cam = Camera.main;

        //show warnings if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
        if (cam == null)
            Debug.LogWarning("Miss Camera for " + stateMachine);
    }

    void OnEnable()
    {
        //check again for camera if deactive and reactive
        if (cam == null) cam = Camera.main;
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (component == null || playerInput == null || playerInput.actions == null)
            return;

        //get input
        inputValue = playerInput.actions.FindAction(inputName).ReadValue<Vector2>();

        //set direction using mouse position
        if (playerInput.currentControlScheme == mouseSchemeName)
        {
            //be sure to have camera setted
            if (cam)
            {
                component.AimAt(cam.ScreenToWorldPoint(inputValue));
            }
        }
        //or using analog
        else
        {
            //check if moving analog or reset input when released
            if (inputValue != Vector2.zero || resetWhenReleaseAnalogInput)
            {
                component.AimAt((Vector2)transformTask.position + inputValue);
                lastAnalogSavedValue = inputValue;  //save input
            }
            //else show last saved input
            else
            {
                component.AimAt((Vector2)transformTask.position + lastAnalogSavedValue);
            }
        }
    }
}
