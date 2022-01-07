using UnityEngine;
using redd096;

public class DustRunFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] MovementComponent movementComponent = default;
    [SerializeField] AimComponent aimComponent = default;

    [Header("Run when speed > this value")]
    [SerializeField] float valueToRun = 0.1f;

    [Header("Dust to activate (already in scene/prefab)")]
    [SerializeField] GameObject dustObject = default;

    [Header("Rotate Dust")]
    [SerializeField] bool defaultAtLeftOfCharacter = true;
    [SerializeField] bool rotateBasedOnMovement = false;
    [SerializeField] bool rotateBasedOnAim = true;

    bool ownerIsLookingRight;

    void Awake()
    {
        //set default value
        ownerIsLookingRight = defaultAtLeftOfCharacter;
    }

    void OnEnable()
    {
        if (movementComponent == null) movementComponent = GetComponentInParent<MovementComponent>();
        if (aimComponent == null) aimComponent = GetComponentInParent<AimComponent>();

        //by default hide dust
        if (dustObject)
            dustObject.SetActive(false);

        //add events
        if(movementComponent)
        {
            movementComponent.onChangeMovementDirection += OnChangeMovementDirection;
            OnChangeMovementDirection(movementComponent.IsMovingRight); //set default value
        }
        if(aimComponent)
        {
            aimComponent.onChangeAimDirection += OnChangeAimDirection;
            OnChangeAimDirection(aimComponent.IsLookingRight);          //set default value
        }
    }

    void OnDisable()
    {
        //remove events
        if (movementComponent)
        {
            movementComponent.onChangeMovementDirection -= OnChangeMovementDirection;
        }
        if (aimComponent)
        {
            aimComponent.onChangeAimDirection -= OnChangeAimDirection;
        }
    }

    void Update()
    {
        if (movementComponent && dustObject)
        {

            //start dust when run
            if (movementComponent.CurrentSpeed > valueToRun && dustObject.activeInHierarchy == false)
            {
                dustObject.SetActive(true);
            }
            //hide dust on idle
            else if (movementComponent.CurrentSpeed <= valueToRun && dustObject.activeInHierarchy)
            {
                dustObject.SetActive(false);
            }
        }
    }

    #region events

    void OnChangeMovementDirection(bool movingRight)
    {
        //rotate based on movement
        if(dustObject && rotateBasedOnMovement)
        {
            if(movingRight != ownerIsLookingRight)
            {
                ownerIsLookingRight = movingRight;

                //rotate scale and change offset
                dustObject.transform.localPosition = new Vector2(-dustObject.transform.localPosition.x, dustObject.transform.localPosition.y);
                dustObject.transform.localScale = new Vector2(-dustObject.transform.localScale.x, dustObject.transform.localScale.y);
            }
        }
    }

    void OnChangeAimDirection(bool lookingRight)
    {
        //rotate based on aim
        if(dustObject && rotateBasedOnAim)
        {
            if (lookingRight != ownerIsLookingRight)
            {
                ownerIsLookingRight = lookingRight;

                //rotate scale and change offset
                dustObject.transform.localPosition = new Vector2(-dustObject.transform.localPosition.x, dustObject.transform.localPosition.y);
                dustObject.transform.localScale = new Vector2(-dustObject.transform.localScale.x, dustObject.transform.localScale.y);
            }
        }
    }

    #endregion
}
