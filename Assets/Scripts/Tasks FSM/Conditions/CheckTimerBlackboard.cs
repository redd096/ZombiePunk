using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Timer Blackboard")]
public class CheckTimerBlackboard : ConditionTask
{
    [Header("Get timer in blackboard")]
    [SerializeField] string timerName = "TimerChase";

    public override bool OnCheckTask()
    {
        //if timer is ended, return true
        return Time.time > stateMachine.GetBlackboardElement<float>(timerName);
    }
}
