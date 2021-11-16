using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/Movement")]
[Description("Get a direction to pass at MovementComponent")]
public class MoveByMovementComponent : ActionTask<MovementComponent>
{
    [BlackboardOnly] public BBParameter<Vector2> inputDirection;
    public BBParameter<float> speed = 5;
    public bool repeat;

    protected override void OnUpdate()
    {
        //pass direction and speed to MovementComponent
        agent.MoveInDirection(inputDirection.value, speed.value);

        //end action if necessary
        if (!repeat) { EndAction(); }
    }
}
