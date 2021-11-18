using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/PathFinding")]
[Description("Set AimDirection to look at next node, or target if there is no path")]
public class AimAtPathOrTarget : ActionTask<AimComponent>
{
    [BlackboardOnly] public BBParameter<List<redd096.Node>> path;
    [BlackboardOnly] public BBParameter<Transform> target;
    public bool repeat;

    protected override void OnUpdate()
    {
        //aim at next node in path, if there is one
        if (path.value != null && path.value.Count > 0)
        {
            agent.AimAt(path.value[0].worldPosition - agent.transform.position);
        }
        //else aim at target
        else if (target.value)
        {
            agent.AimAt(target.value.position - agent.transform.position);
        }

        //end action if necessary
        if (!repeat) { EndAction(); }
    }
}
