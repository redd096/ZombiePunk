using UnityEngine;
using redd096;
using redd096.GameTopDown2D;
using redd096.Attributes;

[AddComponentMenu("redd096/Tasks FSM/Action/Boss/Charge To Random Point")]
public class ChargeToRandomPoint : ActionTask
{
    [Header("Necessary Components - default get in parent or this gameObject")]
    [SerializeField] MovementComponent movementComponent = default;
    [SerializeField] AimComponent aimComponent = default;
    [SerializeField] CheckHitSomething checkHitSomething = default;

    [Header("Movement")]
    [SerializeField] float speedMovement = 5;
    [SerializeField] Vector2 centerArena = Vector2.zero;
    [SerializeField] Vector2 sizeArena = Vector2.one * 3;

    [Header("Number of points to reach")]
    [SerializeField] int numberOfPointsToReach = 5;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;
    [SerializeField] float minDistance = 2;
    [ReadOnly] [SerializeField] int numberOfPointsHitted = 0;

    float leftArena => centerArena.x - sizeArena.x * 0.5f + minDistance;
    float rightArena => centerArena.x + sizeArena.x * 0.5f - minDistance;
    float upArena => centerArena.y + sizeArena.y * 0.5f - minDistance;
    float downArena => centerArena.y - sizeArena.y * 0.5f + minDistance;

    void OnDrawGizmos()
    {
        if (drawDebug)
        {
            //draw arena
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(centerArena, sizeArena);

            //draw arena with min distance calculated
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(centerArena, new Vector2(sizeArena.x - minDistance * 2, sizeArena.y - minDistance * 2));

            //draw line in direction
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transformTask.position, (Vector2)transformTask.position + (aimComponent ? aimComponent.AimDirectionInput : Vector2.right));
            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (movementComponent == null) movementComponent = GetStateMachineComponent<MovementComponent>();
        if (aimComponent == null) aimComponent = GetStateMachineComponent<AimComponent>();
        if (checkHitSomething == null) checkHitSomething = GetComponent<CheckHitSomething>();

        //init checkHit task manually
        if (checkHitSomething)
            checkHitSomething.InitializeTask(stateMachine);
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //reset vars
        numberOfPointsHitted = 0;

        //calculate first direction
        CalculateRandomDirection();
        checkHitSomething.OnEnterTask();    //reset vars on checkHitSomething task
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        //do only until reach number of points to hit
        if (numberOfPointsHitted >= numberOfPointsToReach)
            return;

        //charge
        if (movementComponent)
            movementComponent.MoveInDirection(aimComponent ? aimComponent.AimDirectionInput : Vector2.right, speedMovement);

        //if hit something, calculate new direction
        if (checkHitSomething && checkHitSomething.OnCheckTask())
        {
            CalculateRandomDirection();
            checkHitSomething.OnEnterTask();    //reset vars on checkHitSomething task

            //add one to number of points hitted, and check if completed task
            numberOfPointsHitted++;
            if (numberOfPointsHitted >= numberOfPointsToReach)
            {
                CompleteTask();
            }
        }
    }

    void CalculateRandomDirection()
    {
        //select random direction 
        float x = Random.Range(leftArena, rightArena);
        float y = Random.Range(downArena, upArena);

        //set aim in direction
        if (aimComponent)
            aimComponent.AimAt(new Vector2(x, y));
    }
}
