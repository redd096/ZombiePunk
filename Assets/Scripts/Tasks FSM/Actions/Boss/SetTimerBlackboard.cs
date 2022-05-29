using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/Boss/Set Timer Blackboard")]
public class SetTimerBlackboard : ActionTask
{
    public enum ETimerRule { OnEnter, OnExit }

    [Header("Set timer in blackboard")]
    [SerializeField] string timerName = "TimerChase";
    [SerializeField] float durationTimer = 15;

    [Header("Set OnEnter or OnExit")]
    [SerializeField] ETimerRule timerRule = ETimerRule.OnExit;

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //set timer in blackboard
        if (timerRule == ETimerRule.OnEnter)
            stateMachine.SetBlackboardElement(timerName, Time.time + durationTimer);
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //set timer in blackboard
        if (timerRule == ETimerRule.OnExit)
            stateMachine.SetBlackboardElement(timerName, Time.time + durationTimer);
    }
}
