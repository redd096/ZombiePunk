using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096")]
[Description("Check Distance2D from this transform to Target")]
public class CheckDistance2D : ConditionTask<Transform>
{
    public BBParameter<Transform> target;
    public CompareMethod checkType = CompareMethod.LessThan;
    public BBParameter<float> distance = 10;
    [SliderField(0, 0.1f)]
    public float floatingPoint = 0.05f;

    public override void OnDrawGizmosSelected()
    {
        if (agent != null)
        {
            Gizmos.DrawWireSphere((Vector2)agent.position, distance.value);
        }
    }

    protected override bool OnCheck()
    {
        if (target.value == null)
            return false;

        return OperationTools.Compare(Vector2.Distance(agent.position, target.value.transform.position), distance.value, checkType, floatingPoint);
    }
}
