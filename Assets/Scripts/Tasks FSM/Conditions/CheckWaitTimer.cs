using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Wait Timer")]
public class CheckWaitTimer : ConditionTask
{
    [Header("Wait Timer")]
    [SerializeField] float timer = 1;

    float timeToEnd;

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //set time to end
        timeToEnd = Time.time + timer;
    }

    public override bool OnCheckTask()
    {
        //return when timer is finished
        return Time.time > timeToEnd;
    }
}
