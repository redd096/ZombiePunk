using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/Movement")]
[Description("Move only forward and back with MovementComponent, using direction as forward")]
public class MoveTankSystem : ActionTask<MovementComponent>
{
    [BlackboardOnly] public BBParameter<float> inputMovement;
    public BBParameter<Vector2> forwardDirection = Vector2.up;
    public BBParameter<bool> canMoveBack = false;
    public BBParameter<float> speed = 5;
    public bool repeat;

    protected override void OnUpdate()
    {
        //pass direction and speed to MovementComponent, using direction as forward
        float movementValue = (canMoveBack.value || inputMovement.value > 0) ? inputMovement.value : 0;    //can move forward, and back only if setted
        agent.MoveInDirection(forwardDirection.value * movementValue, speed.value);

        //end action if necessary
        if (!repeat) { EndAction(); }
    }
}
