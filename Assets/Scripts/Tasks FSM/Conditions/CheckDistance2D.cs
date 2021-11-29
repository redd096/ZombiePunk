using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Distance 2D")]
public class CheckDistance2D : ConditionTask
{
    enum ECompareMethod { EqualTo, GreaterThan, LessThan, GreaterOrEqualTo, LessOrEqualTo }

    [Header("Check Distance")]
    [SerializeField] string targetBlackboardName = "Target";
    [SerializeField] ECompareMethod compare = ECompareMethod.GreaterThan;
    [SerializeField] float distance = 7;

    [Header("DEBUG")]
    [SerializeField] bool showDebug = false;
    [Range(0f, 0.1f)] [SerializeField] float equalsCheckThreshold = 0.05f;
    [Tooltip("Return true or false when target is null?")] [SerializeField] bool ifTargetIsNullReturnTrue = false;

    Transform target;

    void OnDrawGizmos()
    {
        //draw radius distance
        if(showDebug)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transformTask.position, distance);
            Gizmos.color = Color.white;
        }
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //get target from blackboard
        target = stateMachine.GetBlackboardElement(targetBlackboardName) as Transform;
    }

    public override bool OnCheckTask()
    {
        //if there is no target, return
        if (target == null)
            return ifTargetIsNullReturnTrue;

        //else return compare
        return Compare(Vector2.Distance(target.position, transformTask.position), distance, compare, equalsCheckThreshold);
    }


    /// <summary>
    /// Compare between 2 values
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="cm"></param>
    /// <param name="floatingPoint"></param>
    /// <returns></returns>
    bool Compare(float a, float b, ECompareMethod cm, float floatingPoint)
    {
        if (cm == ECompareMethod.EqualTo)
            return Mathf.Abs(a - b) <= floatingPoint;
        if (cm == ECompareMethod.GreaterThan)
            return a > b;
        if (cm == ECompareMethod.LessThan)
            return a < b;
        if (cm == ECompareMethod.GreaterOrEqualTo)
            return a >= b;
        if (cm == ECompareMethod.LessOrEqualTo)
            return a <= b;
        return true;
    }
}
