using UnityEngine;
using redd096.GameTopDown2D;
using redd096;

public class SetStateOnDie : MonoBehaviour
{
    HealthComponent healthComponent;
    StateMachineRedd096 stateMachine;

    void OnEnable()
    {
        //get references
        healthComponent = GetComponentInParent<HealthComponent>();
        stateMachine = GetComponentInChildren<StateMachineRedd096>();

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

    private void OnDie(HealthComponent whoDied)
    {
        //set state null, when die
        if (stateMachine)
            stateMachine.SetState(-1);
    }
}
