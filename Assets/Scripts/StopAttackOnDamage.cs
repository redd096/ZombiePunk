using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096;

public class StopAttackOnDamage : MonoBehaviour
{
    [SerializeField] float timeBeforeResetState = 0.2f;

    HealthComponent healthComponent;
    StateMachineRedd096 stateMachine;
    Coroutine resetStateCoroutine;

    void OnEnable()
    {
        //get references
        if (healthComponent == null) healthComponent = GetComponentInParent<HealthComponent>();
        if (stateMachine == null) stateMachine = GetComponentInChildren<StateMachineRedd096>();

        //add events
        if (healthComponent)
        {
            healthComponent.onGetDamage += OnGetDamage;
        }
    }

    void OnDisable()
    {
        //remove events
        if (healthComponent)
        {
            healthComponent.onGetDamage -= OnGetDamage;
        }

        //if disable when coroutine is running, set state
        if (resetStateCoroutine != null)
        {
            if (stateMachine)
                stateMachine.SetState(0);
        }
    }

    void OnGetDamage(Vector2 hitPoint)
    {
        //set state null, when get damage
        if (stateMachine)
        {
            stateMachine.SetState(-1);

            //start coroutine
            if (resetStateCoroutine != null)
                StopCoroutine(resetStateCoroutine);

            resetStateCoroutine = StartCoroutine(ResetStateCoroutine());
        }
    }

    IEnumerator ResetStateCoroutine()
    {
        //wait
        yield return new WaitForSeconds(timeBeforeResetState);

        //and reset to first state
        if (stateMachine)
        {
            stateMachine.SetState(0);
        }

        resetStateCoroutine = null;
    }
}
