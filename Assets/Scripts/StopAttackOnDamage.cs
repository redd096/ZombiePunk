using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096;

public class StopAttackOnDamage : MonoBehaviour
{
    public enum ETypeOfCheck { IgnoreInTheseStates, CheckOnlyInTheseStates }

    [SerializeField] float timeBeforeResetState = 0.2f;

    [Header("Ignore when in these states, or check only in these states")]
    [SerializeField] ETypeOfCheck typeOfCheck = ETypeOfCheck.IgnoreInTheseStates;
    [SerializeField] List<string> states = default;

    HealthComponent healthComponent;
    StateMachineRedd096 stateMachine;
    Coroutine resetStateCoroutine;

    void Awake()
    {
        //remove every white space and set upper case
        for (int i = 0; i < states.Count; i++)
            states[i] = states[i].Replace(" ", "").ToUpper();
    }

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
            if (states.Contains(stateMachine.CurrentState.StateName.Replace(" ", "").ToUpper()))
            {
                //if ignore in these states, and current state is in the list, return
                if (typeOfCheck == ETypeOfCheck.IgnoreInTheseStates)
                    return;
            }
            else
            {
                //if check only in these states, but current state is not in the list, return
                if (typeOfCheck == ETypeOfCheck.CheckOnlyInTheseStates)
                    return;
            }

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
