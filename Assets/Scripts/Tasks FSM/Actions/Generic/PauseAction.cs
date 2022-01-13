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

        //if press input, resume
        if(playerInput.actions.FindAction(buttonName).triggered)
        {
            if (SceneLoader.instance)
                SceneLoader.instance.ResumeGame();
        }
    }
}
