using UnityEngine;
using redd096;
using NaughtyAttributes;

[AddComponentMenu("redd096/Tasks FSM/Action/Movement Normal/Move To Position")]
public class MoveToPosition : ActionTask
{
    [Header("Necessary Components - default get in parent and child")]
    [SerializeField] MovementComponent movementComponent;
    [SerializeField] SpriteRenderer[] spritesToChangeVisiblity = default;

    [Header("Movement")]
    [SerializeField] bool useBlackboard = true;
    [EnableIf("useBlackboard")] [SerializeField] string positionBlackboardName = "Last Target Position";
    [DisableIf("useBlackboard")] [SerializeField] Vector2 positionToReach = Vector2.one;
    [SerializeField] float speedMovement = 5;
    [SerializeField] bool ignoreCollisions = true;

    [Header("On Enter State")]
    [SerializeField] bool setVisibleOnEnterState = false;
    [EnableIf("setVisibleOnEnterState")] [SerializeField] bool valueToSetVisibleOnEnterState = false;

    [Header("When reach destination")]
    [SerializeField] bool setVisibleOnReachDestination = false;
    [EnableIf("setVisibleOnReachDestination")] [SerializeField] bool valueToSetVisibleOnReachDestination = false;

    [Header("On Exit State")]
    [SerializeField] bool setVisibleOnExitState = false;
    [EnableIf("setVisibleOnExitState")] [SerializeField] bool valueToSetVisibleOnExitState = false;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;
    [Range(0f, 0.5f)] [SerializeField] float approxReachNode = 0.05f;

    Vector2 positionFromBlackboard;
    bool reachedPosition;
    Vector2 direction;

    void OnDrawGizmos()
    {
        //draw line to reach position
        if (drawDebug)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transformTask.position, useBlackboard ? positionFromBlackboard : positionToReach);
            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (movementComponent == null) movementComponent = GetStateMachineComponent<MovementComponent>();
        if (spritesToChangeVisiblity == null || spritesToChangeVisiblity.Length <= 0)
        {
            Redd096Main main = GetComponentInParent<Redd096Main>();
            if (main) spritesToChangeVisiblity = main.GetComponentsInChildren<SpriteRenderer>();
        }
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //reset vars
        reachedPosition = false;

        //get position from blackboard
        positionFromBlackboard = stateMachine.GetBlackboardElement<Vector2>(positionBlackboardName);

        //set visibility sprites
        if (setVisibleOnEnterState)
            foreach (SpriteRenderer sprite in spritesToChangeVisiblity)
                sprite.enabled = valueToSetVisibleOnEnterState;
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
        if (Vector2.Distance(transformTask.position, useBlackboard ? positionFromBlackboard : positionToReach) <= approxReachNode)
        {
            OnReachPosition();
        }
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //set visibility sprites
        if (setVisibleOnExitState)
            foreach (SpriteRenderer sprite in spritesToChangeVisiblity)
                sprite.enabled = valueToSetVisibleOnExitState;
    }

    void Move()
    {
        if (movementComponent)
        {
            //calculate direction
            direction = ((useBlackboard ? positionFromBlackboard : positionToReach) - (Vector2)transformTask.position).normalized;

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
            if (Vector2.Distance(((useBlackboard ? positionFromBlackboard : positionToReach) - (Vector2)transformTask.position).normalized, direction) > 0.1f)
                transformTask.position = (useBlackboard ? positionFromBlackboard : positionToReach);
        }
    }

    void OnReachPosition()
    {
        reachedPosition = true;

        //set visiblity sprites
        if (setVisibleOnReachDestination)
            foreach (SpriteRenderer sprite in spritesToChangeVisiblity)
                sprite.enabled = valueToSetVisibleOnReachDestination;
    }
}
