using ParadoxNotion;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096")]
[Description("Check Time.Timescale")]
public class CheckTimeScale : ConditionTask
{
    public CompareMethod compare = CompareMethod.EqualTo;
    public BBParameter<float> value;
    [SliderField(0, 0.1f)]
    public float differenceThreshold = 0.05f;

    protected override bool OnCheck()
    {
        return OperationTools.Compare(Time.timeScale, value.value, compare, differenceThreshold);
    }
}
