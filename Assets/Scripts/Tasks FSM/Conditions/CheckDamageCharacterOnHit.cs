using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096")]
[Description("Return true when DamageCharacterOnHit call event OnHit")]
public class CheckDamageCharacterOnHit : ConditionTask<DamageCharacterOnHit>
{
    bool hitSomething;

    protected override void OnEnable()
    {
        //add events
        agent.onHit += OnHit;
    }

    protected override void OnDisable()
    {
        //remove events
        agent.onHit -= OnHit;
    }

    protected override bool OnCheck()
    {
        //when hit something, reset it but return true
        if(hitSomething)
        {
            hitSomething = false;
            return true;
        }

        return false;
    }

    void OnHit()
    {
        hitSomething = true;
    }
}
