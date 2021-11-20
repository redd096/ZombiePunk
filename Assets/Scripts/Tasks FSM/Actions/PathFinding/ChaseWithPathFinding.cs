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

    redd096.Node lastWalkableNode;

    protected override string OnInit()
    {
        //reset var
        lastWalkableNode = null;
        return base.OnInit();
    }

    protected override void OnUpdate()
    {
        //do only if there is target
        if (target.value == null)
        {
            //end action if necessary
            if (!repeat) { EndAction(); }
            return;
        }

        //update path
        UpdatePath();

        //if there is path, move to next node. When reach node, remove from the list
        if (savePathAs.value != null && savePathAs.value.Count > 0)
        {
            if (lastWalkableNode != null)
                lastWalkableNode = null;

            agent.MoveInDirection((savePathAs.value[0].worldPosition - agent.transform.position).normalized, speedChase.value);
            CheckReachNode();
        }
        //if there is no path, move straight to target
        else
        {
            //save last walkable node
            if (lastWalkableNode == null)
                lastWalkableNode = GameManager.instance.pathFindingAStar.Grid.NodeFromWorldPosition(agent.transform.position);

            //check if player is in neighbours nodes, move straight to him
            redd096.Node targetNode = GameManager.instance.pathFindingAStar.Grid.NodeFromWorldPosition(target.value.position);
            if (GameManager.instance.pathFindingAStar.Grid.GetNeighbours(lastWalkableNode).Contains(targetNode))
            {
                agent.MoveInDirection((target.value.position - agent.transform.position).normalized, speedChase.value);
            }
            //else back to last walkable node
            else
            {
                agent.MoveInDirection((lastWalkableNode.worldPosition - agent.transform.position).normalized, speedChase.value);
            }

        }

        //end action if necessary
        if (!repeat) { EndAction(); }
    }

    #region private API

    void UpdatePath()
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
