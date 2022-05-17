using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Movement Normal/Simple Move To Position")]
public class SimpleMoveToPosition : ActionTask
{
    [Header("Necessary Components - default get in parent and child")]
    [SerializeField] MovementComponent movementComponent;

    [Header("Movement")]
    [SerializeField] Vector2 positionToReach = Vector2.one;
    [SerializeField] float speedMovement = 5;
    [SerializeField] bool ignoreCollisions = true;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;
    [Range(0f, 0.5f)] [SerializeField] float approxReachNode = 0.05f;

    bool reachedPosition;
    Vector2 direction;

    void OnDrawGizmos()
    {
        //draw line to reach position
        if (drawDebug)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transformTask.position, positionToReach);
            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (movementComponent == null) movementComponent = GetStateMachineComponent<MovementComponent>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //reset vars
        reachedPosition = false;
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        //if reached position, do nothing
        if (reachedPosition)
        {
            return;
        }

        //move to position
        Move();

        //set when reach position
        if (Vector2.Distance(transformTask.position, positionToReach) <= approxReachNode)
        {
            OnReachPosition();
        }
    }

    void Move()
    {
        if (movementComponent)
        {
            //calculate direction
            direction = (positionToReach - (Vector2)transformTask.position).normalized;

            //force transform position movement, to ignore collisions
            if (ignoreCollisions)
            {
                movementComponent.transform.position = (Vector2)movementComponent.transform.position + direction * speedMovement * Time.deltaTime;
            }
            //else use movement component normally
            else
            {
                movementComponent.MoveInDirection(direction, speedMovement);
            }

            //if different direction, then we have sorpassed position to reach
            if (Vector2.Distance((positionToReach - (Vector2)transformTask.position).normalized, direction) > 0.1f)
                movementComponent.transform.position = positionToReach;
        }
    }

    void OnReachPosition()
    {
        reachedPosition = true;

        //call complete task event
        CompleteTask();
    }
}
