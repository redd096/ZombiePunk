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

    [Header("Calculate hit only with this layers")]
    [SerializeField] LayerMask layersToCheck = -1;

    [Header("DEBUG")]
    [SerializeField] float timeBeforeStartCheck = 0.2f;

    Dictionary<GameObject, Vector2> hits = new Dictionary<GameObject, Vector2>();
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
        if (hits.ContainsKey(collision.gameObject) == false)
            hits.Add(collision.gameObject, collision.GetContact(0).point);
    }

    void OnOwnerCollisionExit2D(Collision2D collision)
    {
        //remove from hits
        if (hits.ContainsKey(collision.gameObject))
            hits.Remove(collision.gameObject);
    }

    #endregion

    #region private API

    bool CheckEveryHit()
    {
        //check if hit in some direction
        foreach (GameObject hit in hits.Keys)
        {
            //check layer
            if (layersToCheck.ContainsLayer(hit.layer))
                return true;
        }

        //if there aren't hits or there aren't collision with correct layer, return false
        return false;
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
                    foreach (GameObject hit in hits.Keys)
                        if ((hits[hit] - (Vector2)transform.position).normalized.x > 0 && layersToCheck.ContainsLayer(hit.layer))  //check also layer
                            return true;
                }
                //check hits left
                else
                {
                    foreach (GameObject hit in hits.Keys)
                        if ((hits[hit] - (Vector2)transform.position).normalized.x < 0 && layersToCheck.ContainsLayer(hit.layer))  //check also layer
                            return true;
                }
            }
            if (Mathf.Abs(movementComponent.MoveDirectionInput.y) > Mathf.Epsilon)
            {
                //check hit up
                if (movementComponent.MoveDirectionInput.y > 0)
                {
                    foreach (GameObject hit in hits.Keys)
                        if ((hits[hit] - (Vector2)transform.position).normalized.y > 0 && layersToCheck.ContainsLayer(hit.layer))  //check also layer
                            return true;
                }
                //check hit down
                else
                {
                    foreach (GameObject hit in hits.Keys)
                        if ((hits[hit] - (Vector2)transform.position).normalized.y < 0 && layersToCheck.ContainsLayer(hit.layer))  //check also layer
                            return true;
                }
            }
        }

        return false;
    }

    #endregion
}
