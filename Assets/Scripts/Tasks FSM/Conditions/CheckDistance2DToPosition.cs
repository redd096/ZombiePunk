using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Distance 2D To Position")]
public class CheckDistance2DToPosition : ConditionTask
{
    enum ECompareMethod { EqualTo, GreaterThan, LessThan, GreaterOrEqualTo, LessOrEqualTo }

    [Header("Check Distance")]
    [SerializeField] string positionBlackboardName = "Last Target Position";
    [SerializeField] ECompareMethod compare = ECompareMethod.LessThan;
    [SerializeField] float distance = 0.1f;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;
    [Range(0f, 0.1f)] [SerializeField] float equalsCheckThreshold = 0.05f;

    Vector2 positionFromBlackboard;

    void OnDrawGizmos()
    {
        //draw line to reach position
        if (drawDebug)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transformTask.position, positionFromBlackboard);
            Gizmos.color = Color.white;
        }
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //get position from blackboard
        positionFromBlackboard = stateMachine.GetBlackboardElement<Vector2>(positionBlackboardName);
    }

    public override bool OnCheckTask()
    {
        //else return compare
        return Compare(Vector2.Distance(positionFromBlackboard, transformTask.position), distance, compare, equalsCheckThreshold);
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
