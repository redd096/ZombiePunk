using UnityEngine;
using redd096;

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
    }

    public override bool OnCheckTask()
    {
        //before start check, wait timer
        if (timerBeforeCheck > Time.time)
            return false;

        //check hit something
        return checkMovementDirection ? CheckOnlyMovementDirection() : CheckEveryHit();
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
            bool hitHorizontal = false;
            bool hitVertical = false;

            //check hit right or left
            if (Mathf.Abs(movementComponent.MoveDirectionInput.x) > Mathf.Epsilon)
                hitHorizontal = component.IsHitting(movementComponent.MoveDirectionInput.x > 0 ? CollisionComponent.EDirectionEnum.right : CollisionComponent.EDirectionEnum.left);

            //check hit up or down
            if (Mathf.Abs(movementComponent.MoveDirectionInput.y) > Mathf.Epsilon)
                hitVertical = component.IsHitting(movementComponent.MoveDirectionInput.y > 0 ? CollisionComponent.EDirectionEnum.up : CollisionComponent.EDirectionEnum.down);

            return hitHorizontal || hitVertical;
        }

        return true;
    }

    #endregion
}
