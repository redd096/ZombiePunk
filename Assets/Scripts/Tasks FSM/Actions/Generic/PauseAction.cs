using UnityEngine;
using redd096;
using UnityEngine.InputSystem;

[AddComponentMenu("redd096/Tasks FSM/Action/Generic/Pause Action")]
public class PauseAction : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] PlayerInput playerInput;

    [Header("Resume Button")]
    [SerializeField] string buttonName = "Resume";

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

        //show warning if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);

        if (SceneLoader.instance == null)
            Debug.LogWarning("Miss SceneLoader in scene");
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //pause on enter this task
        if (SceneLoader.instance)
        {
            SceneLoader.instance.PauseGame();
        }
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (playerInput == null || playerInput.actions == null)
            return;

        //if press input, resume or close options menu
        if (playerInput.actions.FindAction(buttonName).triggered)
        {
            if (GameManager.instance && GameManager.instance.uiManager && GameManager.instance.uiManager.IsOptionsMenuActive()) //if options menu is active, close
                GameManager.instance.uiManager.OptionsMenu(false);
            else if (SceneLoader.instance)                                                                                      //else resume
                SceneLoader.instance.ResumeGame();
        }
    }
}
