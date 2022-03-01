using UnityEngine;
using redd096;
using redd096.Attributes;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Generic/Set Invincible")]
public class SetInvincible : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] HealthComponent healthComponent;

    [Header("On Enter State")]
    [SerializeField] bool setInvincibleOnEnterState = false;
    [EnableIf("setInvincibleOnEnterState")] [SerializeField] bool valueToSetInvincibleOnEnterState = false;

    [Header("On Exit State")]
    [SerializeField] bool setInvincibleOnExitState = false;
    [EnableIf("setInvincibleOnExitState")] [SerializeField] bool valueToSetInvincibleOnExitState = false;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (healthComponent == null) healthComponent = GetStateMachineComponent<HealthComponent>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //if necessary, set invincible or not
        if (setInvincibleOnEnterState && healthComponent)
            healthComponent.Invincible = valueToSetInvincibleOnEnterState;
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //if necessary, set invincible or not
        if (setInvincibleOnExitState && healthComponent)
            healthComponent.Invincible = valueToSetInvincibleOnExitState;
    }
}
