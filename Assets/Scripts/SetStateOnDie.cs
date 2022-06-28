using UnityEngine;
using redd096.GameTopDown2D;
using redd096;
using redd096.Attributes;

public class SetStateOnDie : MonoBehaviour
{
    [SerializeField] bool setStateByIndex = true;
    [EnableIf("setStateByIndex")] [SerializeField] int stateIndex = -1;
    [DisableIf("setStateByIndex")] [SerializeField] string stateName = "StateName";

    HealthComponent healthComponent;
    StateMachineRedd096 stateMachine;

    void OnEnable()
    {
        //get references
        if (healthComponent == null) healthComponent = GetComponentInParent<HealthComponent>();
        if (stateMachine == null) stateMachine = GetComponentInChildren<StateMachineRedd096>();

        //add events
        if (healthComponent)
        {
            healthComponent.onDie += OnDie;
        }
    }

    void OnDisable()
    {
        //remove events
        if (healthComponent)
        {
            healthComponent.onDie -= OnDie;
        }
    }

    private void OnDie(HealthComponent whoDied, Character whoHit)
    {
        //set state, when die
        if (stateMachine)
        {
            if (setStateByIndex)
                stateMachine.SetState(stateIndex);
            else
                stateMachine.SetState(stateName);
        }
    }
}
