using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Feedbacks/Enemy Attack Animation Feedback")]
public class EnemyAttackAnimationFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in child and parent")]
    [SerializeField] Animator anim = default;
    [SerializeField] StateMachineRedd096 stateMachine = default;

    [Header("StateMachine - Attack State")]
    [SerializeField] string attackStateName = "Attack State";

    [Header("Animator trigger's name")]
    [SerializeField] string triggerOnAttack = "Attack";

    void OnEnable()
    {
        //get references
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (stateMachine == null)
        {
            Redd096Main main = GetComponentInParent<Redd096Main>();
            if (main) stateMachine = main.GetComponentInChildren<StateMachineRedd096>();
        }

        //add events
        if (stateMachine)
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

    void OnSetState(string stateName)
    {
        //if enter in attack state
        if(stateName == attackStateName)
        {
            //set trigger
            if (anim)
                anim.SetTrigger(triggerOnAttack);
        }
    }
}
