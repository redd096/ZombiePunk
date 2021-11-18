using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using redd096;

[Category("redd096/Menus")]
[Description("Enable pause menu and Disable when exit")]
public class PauseAction : ActionTask
{
    //on enable
    protected override void OnExecute()
    {
        SceneLoader.instance.PauseGame();
    }

    //on disable
    protected override void OnStop()
    {
        SceneLoader.instance.ResumeGame();
    }
}
