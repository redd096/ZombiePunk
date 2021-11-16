using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/Aim")]
[Description("Rotate aim direction with AimComponent")]
public class AimTankSystem : ActionTask<AimComponent>
{
    [BlackboardOnly] public BBParameter<float> inputRotation;
    public BBParameter<float> speedRotation = 100;
    public BBParameter<bool> inverse = true;
    public bool repeat;

    protected override string OnInit()
    {
        //by default start looking up
        agent.AimDirectionInput = Vector2.up;
        return base.OnInit();
    }

    protected override void OnUpdate()
    {
        //rotate aim direction and set in AimComponent
        Vector2 aimDirection = Quaternion.AngleAxis(speedRotation.value * Time.deltaTime, GetRotationAxis() * GetIfInverse()) * agent.AimDirectionInput;
        agent.AimAt(aimDirection);

        //end action if necessary
        if (!repeat) { EndAction(); }
    }

    Vector3 GetRotationAxis()
    {
        //rotation axis is forward or backward, based on input
        return Vector3.forward * inputRotation.value;
    }

    int GetIfInverse()
    {
        //if inverse, reverse the rotation axis
        return inverse.value ? -1 : 1;
    }
}
