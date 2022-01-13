using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/Aim/Aim From Top")]
public class AimFromTop : ActionTask
{
    [Header("Aim To Target")]
    [SerializeField] string targetBlackboardName = "Target";
    [SerializeField] float durationFollowTarget = 2;
    [SerializeField] bool setXPositionSameAsTarget = true;
    [SerializeField] string saveLastTargetPositionInBlackboardAs = "Last Target Position";

    Redd096Main selfOwner;
    Transform target;
    float timerTask;

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //get references
        selfOwner = GetComponentInParent<Redd096Main>();

        //get target from blackboard
        target = stateMachine.GetBlackboardElement<Transform>(targetBlackboardName);

        //set timer
        timerTask = Time.time + durationFollowTarget;
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        //when finish timer, do nothing
        if (Time.time > timerTask)
            return;

        //update last target position
        if (target)
        {
            stateMachine.SetBlackboardElement(saveLastTargetPositionInBlackboardAs, (Vector2)target.position);

            //if necessary, follow x position
            if(setXPositionSameAsTarget)
            {
                selfOwner.transform.position = new Vector2(target.position.x, selfOwner.transform.position.y);
            }
        }
    }
}
