using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Feedbacks/Enemy Charger Animation Feedback")]
public class EnemyChargerAnimationFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in child and parent")]
    [SerializeField] Animator anim = default;
    [SerializeField] MovementComponent movementComponent = default;
    [SerializeField] StateMachineRedd096 stateMachine = default;

    [Header("Run when speed > this value")]
    [SerializeField] float valueToRun = 0.1f;

    [Header("Animator parameters")]
    [SerializeField] string intName = "State";
    [SerializeField] int idleIndex = 0;
    [SerializeField] int patrolIndex = 1;
    [SerializeField] int chargeIndex = 2;
    [SerializeField] int stunIndex = 3;
    [SerializeField] int delayIndex = 4;

    string currentState;
    int currentAnimation;

    void OnEnable()
    {
        //get references
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (movementComponent == null) movementComponent = GetComponentInParent<MovementComponent>();
        if (stateMachine == null)
        {
            Redd096Main main = GetComponentInParent<Redd096Main>();
            if (main) stateMachine = main.GetComponentInChildren<StateMachineRedd096>();
        }

        //add events
        if(stateMachine)
        {
            stateMachine.onSetState += OnSetState;
        }
    }

    void OnDisable()
    {
        //remove events
        if (stateMachine)
        {
            stateMachine.onSetState -= OnSetState;
        }
    }

    void Update()
    {
        //do animations based on state
        if (currentState.Equals("Patrol State"))
            PatrolState();
        else if (currentState.Equals("Charge State"))
            ChargeState();
        else if (currentState.Equals("Stun State"))
            StunState();
        else
            DelayState();
    }

    void OnSetState(string stateName)
    {
        //save current state name
        currentState = stateName;
    }

    #region private API

    void PatrolState()
    {
        if (anim && movementComponent)
        {
            //start run
            if (movementComponent.CurrentSpeed > valueToRun && currentAnimation != patrolIndex)
            {
                currentAnimation = patrolIndex;

                //set animator
                anim.SetInteger(intName, patrolIndex);
            }
            //back to idle
            else if (movementComponent.CurrentSpeed <= valueToRun && currentAnimation != idleIndex)
            {
                currentAnimation = idleIndex;

                //set animator
                anim.SetInteger(intName, idleIndex);
            }
        }
    }

    void ChargeState()
    {
        if(anim)
        {
            currentAnimation = chargeIndex;

            //set charge state
            anim.SetInteger(intName, chargeIndex);
        }
    }

    void StunState()
    {
        if (anim)
        {
            currentAnimation = stunIndex;

            //set stun state
            anim.SetInteger(intName, stunIndex);
        }
    }

    void DelayState()
    {
        if (anim)
        {
            currentAnimation = delayIndex;

            //set delay state
            anim.SetInteger(intName, delayIndex);
        }
    }

    #endregion
}
