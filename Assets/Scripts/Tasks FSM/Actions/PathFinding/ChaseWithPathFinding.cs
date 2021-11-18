using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/PathFinding")]
[Description("Patrol inside a radius using MovementComponent and PathFinding")]
public class ChaseWithPathFinding : ActionTask<MovementComponent>
{
    [BlackboardOnly] public BBParameter<Transform> target;
    public BBParameter<float> speedChase = 5;
    public BBParameter<float> approxReachNode = 0.05f;
    [BlackboardOnly] public BBParameter<List<redd096.Node>> savePathAs;
    public bool repeat;

    protected override void OnUpdate()
    {
        //do only if there is target
        if (target.value == null)
        {
            //end action if necessary
            if (!repeat) { EndAction(); }
            return;
        }

        //if there is no path, find new one
        if (savePathAs.value == null || savePathAs.value.Count <= 0)
        {
            FindNewPath();
        }

        //if there is path, move to next node. When reach node, remove from the list
        if(savePathAs.value != null && savePathAs.value.Count > 0)
        {
            agent.MoveInDirection((savePathAs.value[0].worldPosition - agent.transform.position).normalized, speedChase.value);
            CheckReachNode();
        }
        //if there is no path, move straight to target
        else
        {
            agent.MoveInDirection((target.value.position - agent.transform.position).normalized, speedChase.value);
        }

        //end action if necessary
        if (!repeat) { EndAction(); }
    }

    #region private API

    void FindNewPath()
    {
        //get path
        savePathAs.value = GameManager.instance.pathFindingAStar.FindPath(agent.transform.position, target.value.position);
    }

    void CheckReachNode()
    {
        //if reach node, remove from list
        if (Vector2.Distance(agent.transform.position, savePathAs.value[0].worldPosition) <= approxReachNode.value)
        {
            savePathAs.value.RemoveAt(0);
        }
    }

    #endregion
}
