using UnityEngine;
using redd096;
using redd096.GameTopDown2D;
using System.Collections.Generic;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Hit Something")]
public class CheckHitSomething : ConditionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] CollisionEventToChilds collisionEventToChilds = default;
    [SerializeField] MovementComponent movementComponent;

    [Header("Check only in movement direction")]
    [SerializeField] bool checkMovementDirection = true;

    [Header("DEBUG")]
    [SerializeField] float timeBeforeStartCheck = 0.2f;

    List<Collision2D> hits = new List<Collision2D>();
    float timerBeforeCheck;

    void OnEnable()
    {
        //get references
        if (collisionEventToChilds == null)
            collisionEventToChilds = GetStateMachineComponent<CollisionEventToChilds>();

        //add events
        if (collisionEventToChilds)
        {
            collisionEventToChilds.onCollisionEnter2D += OnOwnerCollisionEnter2D;
            collisionEventToChilds.onCollisionExit2D += OnOwnerCollisionExit2D;
        }
    }

    void OnDisable()
    {
        //remove events
        if (collisionEventToChilds)
        {
            collisionEventToChilds.onCollisionEnter2D -= OnOwnerCollisionEnter2D;
            collisionEventToChilds.onCollisionExit2D -= OnOwnerCollisionExit2D;
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

    #region events

    void OnOwnerCollisionEnter2D(Collision2D collision)
    {
        //add to hits
        if (hits.Contains(collision) == false)
            hits.Add(collision);
    }

    void OnOwnerCollisionExit2D(Collision2D collision)
    {
        //remove from hits
        if (hits.Contains(collision))
            hits.Remove(collision);
    }

    #endregion

    #region private API

    bool CheckEveryHit()
    {
        //check if hit in some direction
        return hits.Count > 0;
    }

    bool CheckOnlyMovementDirection()
    {
        if(movementComponent)
        {
            if (Mathf.Abs(movementComponent.MoveDirectionInput.x) > Mathf.Epsilon)
            {
                //check hit right
                if (movementComponent.MoveDirectionInput.x > 0)
                {
                    foreach (Collision2D col in hits)
                        if ((col.GetContact(0).point - (Vector2)transform.position).normalized.x > 0)
                            return true;
                }
                //check hits left
                else
                {
                    foreach (Collision2D col in hits)
                        if ((col.GetContact(0).point - (Vector2)transform.position).normalized.x < 0)
                            return true;
                }
            }
            if (Mathf.Abs(movementComponent.MoveDirectionInput.y) > Mathf.Epsilon)
            {
                //check hit up
                if (movementComponent.MoveDirectionInput.y > 0)
                {
                    foreach (Collision2D col in hits)
                        if ((col.GetContact(0).point - (Vector2)transform.position).normalized.y > 0)
                            return true;
                }
                //check hit down
                else
                {
                    foreach (Collision2D col in hits)
                        if ((col.GetContact(0).point - (Vector2)transform.position).normalized.y < 0)
                            return true;
                }
            }
        }

        return false;
    }

    #endregion
}
