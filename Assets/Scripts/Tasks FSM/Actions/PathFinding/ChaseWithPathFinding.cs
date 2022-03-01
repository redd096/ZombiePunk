using UnityEngine;
using redd096;
using redd096.PathFinding2D;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/PathFinding/Chase With Path Finding")]
public class ChaseWithPathFinding : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] MovementComponent component;
    [SerializeField] AimComponent aimComponent;
    [SerializeField] AgentAStar2D agentAStar;

    [Header("Chase")]
    [SerializeField] string targetBlackboardName = "Target";
    [SerializeField] float speedChase = 5;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;
    [Range(0f, 0.5f)] [SerializeField] float approxReachNode = 0.05f;
    [SerializeField] float delayRecalculatePath = 0.5f;

    float timerBeforeNextUpdatePath;
    Transform target;
    Path path;
    //Node2D lastWalkableNode;

    void OnDrawGizmos()
    {
        //draw radius patrol
        if (drawDebug)
        {
            //draw path
            if (path != null && path.vectorPath != null && path.vectorPath.Count > 0)
            {
                Gizmos.color = Color.magenta;
                for (int i = 0; i < path.vectorPath.Count; i++)
                {
                    if (i + 1 < path.vectorPath.Count)
                        Gizmos.DrawLine(path.vectorPath[i], path.vectorPath[i + 1]);
                }
                Gizmos.color = Color.white;
            }
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetStateMachineComponent<MovementComponent>();
        if (aimComponent == null) aimComponent = GetStateMachineComponent<AimComponent>();
        if (agentAStar == null) agentAStar = GetStateMachineComponent<AgentAStar2D>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //get target from blackboard
        target = stateMachine.GetBlackboardElement<Transform>(targetBlackboardName);

        //stop previous path request
        if (agentAStar && agentAStar.IsDone() == false)
            agentAStar.CancelLastPathRequest();

        //remove previous path
        if (path != null)
            path = null;
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (target == null)
            return;

        //update path to target
        UpdatePath();

        //if there is path, move to next node
        if (path != null && path.vectorPath != null && path.vectorPath.Count > 0)
        {
            ////if on a walkable node, save it
            //Node2D currentNode = pathFinding.Grid.NodeFromWorldPosition(transformTask.position);
            //if (currentNode.isWalkable)
            //    lastWalkableNode = currentNode;

            //move and aim to next node
            MoveAndAim();
            CheckReachNode();
        }
        ////if there is no path, move straight to target (only if last walkable node is setted)
        //else if(lastWalkableNode != null)
        //{
        //    //if target is in a not walkable node, but neighbour of our last walkable node, move straight to it
        //    Node2D targetNode = pathFinding.Grid.NodeFromWorldPosition(target.position);
        //    if (pathFinding.Grid.GetNeighbours(lastWalkableNode).Contains(targetNode))
        //    {
        //        MoveAndAim(target.position);
        //    }
        //    //else move back to last walkable node
        //    else
        //    {
        //        MoveAndAim(lastWalkableNode.worldPosition);
        //    }
        //}
    }

    #region private API

    void UpdatePath()
    {
        //delay between every update of the path (every few seconds, only if already calculated previous path)
        if (Time.time > timerBeforeNextUpdatePath && agentAStar && agentAStar.IsDone())
        {
            //reset timer
            timerBeforeNextUpdatePath = Time.time + delayRecalculatePath;

            //get path
            agentAStar.FindPath(transformTask.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path path)
    {
        //set path
        this.path = path;
    }

    void MoveAndAim()
    {
        //move to destination
        if (component)
            component.MoveTo(path.vectorPath[0], speedChase);

        //aim at destination
        if (aimComponent)
            aimComponent.AimAt(path.vectorPath[0]);
    }

    void CheckReachNode()
    {
        //if reach node, remove from list
        if (Vector2.Distance(transformTask.position, path.vectorPath[0]) <= approxReachNode)
        {
            path.vectorPath.RemoveAt(0);
        }
    }

    #endregion
}
