using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Damage Character On Hit")]
public class CheckDamageCharacterOnHit : ConditionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] DamageCharacterOnHit component;

    bool hitSomething;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetStateMachineComponent<DamageCharacterOnHit>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //reset bool
        hitSomething = false;

        //add events
        if(component)
            component.onHit += OnHit;
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //remove events
        if(component)
            component.onHit -= OnHit;
    }

    public override bool OnCheckTask()
    {
        //return true when hit something
        return hitSomething;
    }

    void OnHit()
    {
        hitSomething = true;
    }
}
