using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/Movement Normal/Move Straight Forward")]
public class MoveStraightForward : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] MovementComponent component;
    [SerializeField] AimComponent aimComponent;

    [Header("Movement")]
    [SerializeField] string targetBlackboardName = "Target";
    [SerializeField] float speedMovement = 5;
    [Min(0)] [SerializeField] float rotationSpeed = 0;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    Transform target;

    void OnDrawGizmos()
    {
        //draw line to reach position and to target
        if (drawDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transformTask.position, Application.isPlaying && aimComponent ? (Vector2)transformTask.position + aimComponent.AimDirectionInput * 2 : (Vector2)transformTask.position + Vector2.right * 2);
            Gizmos.color = Color.cyan;
            if (target) Gizmos.DrawLine(transformTask.position, target.position);
            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetStateMachineComponent<MovementComponent>();
        if (aimComponent == null) aimComponent = GetStateMachineComponent<AimComponent>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //aim at target
        target = stateMachine.GetBlackboardElement(targetBlackboardName) as Transform;  //get target from blackboard
        if (aimComponent && target) aimComponent.AimAt(target.position);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        //move forward
        if (component)
            component.MoveInDirection(aimComponent ? aimComponent.AimDirectionInput : Vector2.right, speedMovement);

        //rotate aim direction
        if(rotationSpeed > Mathf.Epsilon)
        {
            if (target && aimComponent)
            {
                //calculate rotation
                float angle = Vector2.SignedAngle(aimComponent.AimDirectionInput, (target.position - transformTask.position).normalized);               //angle from aim to target
                float rotationAngle = rotationSpeed * Time.deltaTime > Mathf.Abs(angle) ? angle : rotationSpeed * Time.deltaTime * Mathf.Sign(angle);   //clamp
                Vector2 newAimPosition = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * aimComponent.AimDirectionInput;                         //rotate

                //set new aim position
                aimComponent.AimAt((Vector2)transformTask.position + newAimPosition);
            }
        }
    }
}
