using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/Pause Action")]
public class PauseAction : ActionTask
{
    protected override void OnInitTask()
    {
        base.OnInitTask();

        if (SceneLoader.instance == null)
            Debug.LogWarning("Miss SceneLoader in scene");
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //pause on enter this task
        if (SceneLoader.instance)
            SceneLoader.instance.PauseGame();
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //resume on exit from this task
        if (SceneLoader.instance)
            SceneLoader.instance.ResumeGame();
    }
}
