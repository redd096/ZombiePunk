using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Hit Something")]
public class CheckHitSomething : ConditionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] CollisionComponent component;
    [SerializeField] MovementComponent movementComponent;

    [Header("Check only in movement direction")]
    [SerializeField] bool checkMovementDirection = true;

    [Header("DEBUG")]
    [SerializeField] float timeBeforeStartCheck = 0.2f;

    float timerBeforeCheck;
    bool collisionComponentWasSettedToNone;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetStateMachineComponent<CollisionComponent>();
        if (movementComponent == null) movementComponent = GetStateMachineComponent<MovementComponent>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //set timer before start check
        timerBeforeCheck = Time.time + timeBeforeStartCheck;

        //if collision component has update mode to None, set to Coroutine
        if (component && component.UpdateMode == CollisionComponent.EUpdateModes.None)
        {
            collisionComponentWasSettedToNone = true;                           //save before was setted to None
            component.UpdateMode = CollisionComponent.EUpdateModes.Coroutine;
        }
    }

    public override bool OnCheckTask()
    {
        //before start check, wait timer
        if (timerBeforeCheck > Time.time)
            return false;

        //check hit something
        return checkMovementDirection ? CheckOnlyMovementDirection() : CheckEveryHit();
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //if collision component had update mode to None, set it
        if (collisionComponentWasSettedToNone && component && component.UpdateMode != CollisionComponent.EUpdateModes.None)
        {
            collisionComponentWasSettedToNone = false;
            component.UpdateMode = CollisionComponent.EUpdateModes.None;
            component.ClearHits();                                              //clear every hit registered during this task
        }
    }

    #region private API

    bool CheckEveryHit()
    {
        //check if hit in some direction
        if(component)
        {
            return component.IsHitting(CollisionComponent.EDirectionEnum.up) || component.IsHitting(CollisionComponent.EDirectionEnum.down)
                || component.IsHitting(CollisionComponent.EDirectionEnum.right) || component.IsHitting(CollisionComponent.EDirectionEnum.left);
        }

        return true;
    }

    bool CheckOnlyMovementDirection()
    {
        if(component && movementComponent)
        {
            //check hit right or left
            if (Mathf.Abs(movementComponent.MoveDirectionInput.x) > Mathf.Epsilon)
                if (component.IsHitting(movementComponent.MoveDirectionInput.x > 0 ? CollisionComponent.EDirectionEnum.right : CollisionComponent.EDirectionEnum.left))
                    return true;

            //check hit up or down
            if (Mathf.Abs(movementComponent.MoveDirectionInput.y) > Mathf.Epsilon)
                if (component.IsHitting(movementComponent.MoveDirectionInput.y > 0 ? CollisionComponent.EDirectionEnum.up : CollisionComponent.EDirectionEnum.down))
                    return true;

            return false;
        }

        return true;
    }

    #endregion
}
