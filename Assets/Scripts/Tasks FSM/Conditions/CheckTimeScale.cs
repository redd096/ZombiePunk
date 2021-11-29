using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Time Scale")]
public class CheckTimeScale : ConditionTask
{
    enum ECompareMethod { EqualTo, GreaterThan, LessThan, GreaterOrEqualTo, LessOrEqualTo }

    [Header("TimeScale")]
    [SerializeField] ECompareMethod compare = ECompareMethod.GreaterThan;
    [Range(0f, 1f)] [SerializeField] float value = 0;

    [Header("DEBUG")]
    [Range(0f, 0.1f)] [SerializeField] float equalsCheckThreshold = 0.05f;

    public override bool OnCheckTask()
    {
        //compare
        return Compare(Time.timeScale, value, compare, equalsCheckThreshold);
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
