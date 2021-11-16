using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/Aim")]
[Description("Get a position to pass at AimComponent")]
public class PlayerAimByAimComponent : ActionTask<AimComponent>
{
    [ParadoxNotion.Design.Header("Control Scheme")]
    public BBParameter<string> mouseSchemeName = "KeyboardAndMouse";
    [BlackboardOnly] public BBParameter<string> currentControlScheme;

    [ParadoxNotion.Design.Header("Input Checks")]
    public BBParameter<Camera> cameraForMouseInput = default;
    public BBParameter<bool> resetWhenReleaseAnalogInput = false;

    [ParadoxNotion.Design.Header("Input")]
    [BlackboardOnly] public BBParameter<Vector2> inputPosition;

    [ParadoxNotion.Design.Header("Repeat")]
    public bool repeat;

    protected override void OnUpdate()
    {
        //set direction using mouse position
        if(currentControlScheme.value == mouseSchemeName.value)
        {
            //be sure to have camera setted
            if (cameraForMouseInput.value)
            {
                Vector2 mousePosition = cameraForMouseInput.value.ScreenToWorldPoint(inputPosition.value);
                agent.AimAt(mousePosition - (Vector2)agent.transform.position);
            }
        }
        //or using analog
        else
        {
            //check if reset input when released
            if (inputPosition.value != Vector2.zero || resetWhenReleaseAnalogInput.value)
                agent.AimAt(inputPosition.value);
        }

        //end action if necessary
        if (!repeat) { EndAction(); }
    }
}
