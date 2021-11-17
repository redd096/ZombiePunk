using UnityEngine;
using Sirenix.OdinInspector;

public class MovementComponent : MonoBehaviour
{
    enum EUpdateModes { Update, FixedUpdate }
    enum EMovementModes { Transform, Rigidbody }

    [Header("Movement")]
    [Tooltip("Move on Update or FixedUpdate?")] [SerializeField] EUpdateModes updateMode = EUpdateModes.Update;
    [Tooltip("Move using transform or rigidbody?")] [SerializeField] EMovementModes movementMode = EMovementModes.Transform;
    [Tooltip("Max Speed, calculating velocity by input + push (-1 = no limit)")] [SerializeField] float maxSpeed = 50;

    [Header("When pushed")]
    [Tooltip("Drag used when pushed from something")] [SerializeField] float drag = 30;
    [Tooltip("When player is pushed for example to the right, and hit a wall. Set Push to right at 0?")] [SerializeField] bool removePushForceWhenHit = true;

    [Header("Necessary Components (by default get from this gameObject)")]
    [SerializeField] Rigidbody2D rb = default;
    [SerializeField] CollisionDetector collisionDetector = default;

    [Header("DEBUG")]
    [ReadOnly] public bool IsMovingRight = true;            //check if moving right
    [ReadOnly] public Vector2 MoveDirectionInput;           //when moves, set it with only direction (used to know last movement direction)
    [ReadOnly] public Vector2 DesiredVelocity;              //when moves, set it as direction * speed (used to move this object, will be reset in every frame)
    [ReadOnly] public Vector2 DesiredPushForce;             //used to push this object (push by recoil, knockback, dash, etc...), will be decreased by drag in every frame
    [ReadOnly] public Vector2 CurrentVelocity;              //velocity calculate for this frame
    [ReadOnly] public float CurrentSpeed;                   //CurrentVelocity.magnitude

    //events
    public System.Action onChangeMovementDirection { get; set; }

    void Update()
    {
        //do only if update mode is Update
        if (updateMode == EUpdateModes.Update)
            Move();
    }

    void FixedUpdate()
    {
        //do only if update mode is FixedUpdate
        if (updateMode == EUpdateModes.FixedUpdate)
            Move();
    }

    void Move()
    {
        //start only if there are all necessary components
        if (CheckComponents() == false)
            return;

        //set velocity (input + push + check collisions)
        CurrentVelocity = CalculateVelocity();
        CurrentSpeed = CurrentVelocity.magnitude;

        //set if change movement direction
        if (IsMovingRight != CheckIsMovingRight())
        {
            IsMovingRight = CheckIsMovingRight();

            //call event
            onChangeMovementDirection?.Invoke();
        }

        //do movement
        DoMovement();

        //reset movement input (cause if nobody call an update, this object must to be still in next frame)
        DesiredVelocity = Vector2.zero;

        //remove push force (direction * drag * delta)
        DesiredPushForce = CalculateNewPushForce();
    }

    #region private API

    bool CheckComponents()
    {
        //check if have a collision detector
        if (collisionDetector == null)
            collisionDetector = GetComponent<CollisionDetector>();

        //if movement mode is rigidbody, be sure to have a rigidbody
        if (movementMode == EMovementModes.Rigidbody && rb == null)
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogWarning("Miss Rigidbody on " + name);
                return false;
            }
        }

        return true;
    }

    Vector2 CalculateVelocity()
    {
        //input + push
        Vector2 velocity = DesiredVelocity + DesiredPushForce;

        if (collisionDetector)
        {
            //check if hit horizontal
            if (collisionDetector.IsHitting(CollisionDetector.EDirectionEnum.right) && velocity.x > 0)
                velocity.x = 0;
            else if (collisionDetector.IsHitting(CollisionDetector.EDirectionEnum.left) && velocity.x < 0)
                velocity.x = 0;

            //check if hit vertical
            if (collisionDetector.IsHitting(CollisionDetector.EDirectionEnum.up) && velocity.y > 0)
                velocity.y = 0;
            else if (collisionDetector.IsHitting(CollisionDetector.EDirectionEnum.down) && velocity.y < 0)
                velocity.y = 0;
        }

        //clamp at max speed
        if (maxSpeed >= 0)
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        return velocity;
    }

    bool CheckIsMovingRight()
    {
        //check if change direction
        if (IsMovingRight && CurrentVelocity.x < 0)
            return false;
        else if (IsMovingRight == false && CurrentVelocity.x > 0)
            return true;

        //else return previous direction (necessary in case this object stay still)
        return IsMovingRight;
    }

    void DoMovement()
    {
        //do movement with rigidbody or transform
        if (movementMode == EMovementModes.Rigidbody)
            rb.velocity = CurrentVelocity;
        else if (movementMode == EMovementModes.Transform)
            transform.position += (Vector3)CurrentVelocity * (updateMode == EUpdateModes.Update ? Time.deltaTime : Time.fixedDeltaTime);

        //adjust position to not enter in a collision
        if (collisionDetector)
        {
            AdjustPosition(CollisionDetector.EDirectionEnum.right, Vector2.right, CollisionDetector.EDirectionEnum.left);
            AdjustPosition(CollisionDetector.EDirectionEnum.left, Vector2.left, CollisionDetector.EDirectionEnum.right);
            AdjustPosition(CollisionDetector.EDirectionEnum.up, Vector2.up, CollisionDetector.EDirectionEnum.down);
            AdjustPosition(CollisionDetector.EDirectionEnum.down, Vector2.down, CollisionDetector.EDirectionEnum.up);
        }
    }

    void AdjustPosition(CollisionDetector.EDirectionEnum direction, Vector2 directionMovement, CollisionDetector.EDirectionEnum oppositeDirection)
    {
        //do only if colliding with something, and not colliding in opposite direction
        if (collisionDetector == null || collisionDetector.IsHitting(direction) == false || collisionDetector.IsHitting(oppositeDirection))
            return;

        float bounds = collisionDetector.GetBounds(direction);
        float greaterDistance = 0;

        //find greater distance from hits
        foreach (RaycastHit2D hit in collisionDetector.GetHits(direction))
        {
            //calculate horizontal
            if (Mathf.Abs(directionMovement.x) > Mathf.Abs(directionMovement.y))
            {
                if (bounds - hit.point.x > greaterDistance)
                    greaterDistance = bounds - hit.point.x;
            }
            //calculate vertical
            else
            {
                if (bounds - hit.point.y > greaterDistance)
                    greaterDistance = bounds - hit.point.y;
            }
        }

        //adjust position
        transform.position -= (Vector3)directionMovement * greaterDistance;
    }

    Vector2 CalculateNewPushForce()
    {
        //remove push force (direction * drag * delta)
        Vector2 newPushForce = DesiredPushForce - (DesiredPushForce.normalized * drag * (updateMode == EUpdateModes.Update ? Time.deltaTime : Time.fixedDeltaTime));

        //clamp it
        if (DesiredPushForce.x >= 0 && newPushForce.x < 0 || DesiredPushForce.x <= 0 && newPushForce.x > 0)
            newPushForce.x = 0;
        if (DesiredPushForce.y >= 0 && newPushForce.y < 0 || DesiredPushForce.y <= 0 && newPushForce.y > 0)
            newPushForce.y = 0;

        //check if hit walls
        if (collisionDetector && removePushForceWhenHit)
        {
            //check if hit horizontal
            if (collisionDetector.IsHitting(CollisionDetector.EDirectionEnum.right) && newPushForce.x > 0)
                newPushForce.x = 0;
            else if (collisionDetector.IsHitting(CollisionDetector.EDirectionEnum.left) && newPushForce.x < 0)
                newPushForce.x = 0;

            //check if hit vertical
            if (collisionDetector.IsHitting(CollisionDetector.EDirectionEnum.up) && newPushForce.y > 0)
                newPushForce.y = 0;
            else if (collisionDetector.IsHitting(CollisionDetector.EDirectionEnum.down) && newPushForce.y < 0)
                newPushForce.y = 0;
        }

        return newPushForce;
    }

    #endregion

    #region public API

    /// <summary>
    /// Set movement in direction using custom speed
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="newSpeed"></param>
    public void MoveInDirection(Vector2 direction, float newSpeed)
    {
        //save last input direction + set movement
        MoveDirectionInput = direction;
        DesiredVelocity = direction * newSpeed;
    }

    /// <summary>
    /// Push in direction
    /// </summary>
    /// <param name="pushDirection"></param>
    /// <param name="pushForce"></param>
    /// <param name="resetPreviousPush"></param>
    public virtual void PushInDirection(Vector2 pushDirection, float pushForce, bool resetPreviousPush = false)
    {
        //reset previous push or add new one to it
        if (resetPreviousPush)
            DesiredPushForce = pushDirection * pushForce;
        else
            DesiredPushForce += pushDirection * pushForce;
    }

    #endregion
}
