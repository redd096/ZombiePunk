using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Can See Target 2D")]
public class CheckCanSeeTarget2D : ConditionTask
{
    [Header("Can See Component 2D")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float awarnessDistance = 5;
    [SerializeField] LayerMask layerWalls;
    [SerializeField] string saveTargetInBlackboardAs = "Target";

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    List<Transform> possibleTargets = new List<Transform>();

    void OnDrawGizmos()
    {
        if(drawDebug)
        {
            Gizmos.color = Color.red;

            //draw awareness
            Gizmos.DrawWireSphere(transformTask.position, awarnessDistance);

            Gizmos.color = Color.white;
        }
    }

    public override bool OnCheckTask()
    {
        //find every targets inside max distance
        FindPossibleTargets();

        foreach (Transform t in new List<Transform>(possibleTargets))
        {
            //remove targets if can't see them
            if (IsViewClear(t) == false)
            {
                possibleTargets.Remove(t);
            }
        }

        if (possibleTargets.Count <= 0)
            return false;

        //if found targets, save nearest in the blackboard
        Transform target = GetNearest();
        if (stateMachine.GetBlackboardElement(saveTargetInBlackboardAs) as Transform != target) //save only if not already in the blackboard (to call set only one time - necessary for feedbacks)
            stateMachine.SetBlackboardElement(saveTargetInBlackboardAs, target);

        return true;
    }

    #region private API

    void FindPossibleTargets()
    {
        //clear list
        possibleTargets.Clear();

        //find every element in distance, using layer
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transformTask.position, awarnessDistance, targetLayer.value))
        {
            possibleTargets.Add(col.transform);
        }
    }

    bool IsViewClear(Transform t)
    {
        //check there is nothing between
        return Physics2D.Linecast(transformTask.position, t.position, layerWalls.value) == false;
    }

    Transform GetNearest()
    {
        float distance = Mathf.Infinity;
        Transform nearest = null;

        //find nearest
        foreach (Transform t in possibleTargets)
        {
            if (Vector2.Distance(transformTask.position, t.position) < distance)
            {
                distance = Vector2.Distance(transformTask.position, t.position);
                nearest = t;
            }
        }

        return nearest;
    }

    #endregion
}
