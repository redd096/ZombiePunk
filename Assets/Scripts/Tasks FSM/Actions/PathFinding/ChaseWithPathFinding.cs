using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/PathFinding/Chase With Path Finding")]
public class ChaseWithPathFinding : ActionTask
{
    [Header("Necessary Components - default get in parent or GameManager")]
    [SerializeField] MovementComponent component;
    [SerializeField] AimComponent aimComponent;
    [SerializeField] AgentAStar2D agentAStar;

    [Header("Chase")]
    [SerializeField] string targetBlackboardName = "Target";
    [SerializeField] float speedChase = 5;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;
    [Range(0f, 0.5f)] [SerializeField] float approxReachNode = 0.05f;
    [SerializeField] float delayRecalculatePath = 0.2f;

    float timerBeforeNextUpdatePath;
    Transform target;
    List<Node2D> path;
    bool isPathProcessing;
    //Node2D lastWalkableNode;

    void OnDrawGizmos()
    {
        //draw radius patrol
        if (drawDebug)
        {
            //draw path
            if (path != null && path.Count > 0)
            {
                Gizmos.color = Color.magenta;
                for (int i = 0; i < path.Count; i++)
                {
                    if (i + 1 < path.Count)
                        Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
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

        //show warnings if not found
        if (GameManager.instance == null)
            Debug.LogWarning("Miss GameManager in scene");
        else if (PathFindingAStar2D.instance == null)
            Debug.LogWarning("Miss PathFinding in scene");
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //get target from blackboard
        target = stateMachine.GetBlackboardElement<Transform>(targetBlackboardName);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (target == null)
            return;

        //update path to target
        UpdatePath();

        //if there is path, move to next node
        if (path != null && path.Count > 0)
        {
            ////if on a walkable node, save it
            //Node2D currentNode = pathFinding.Grid.NodeFromWorldPosition(transformTask.position);
            //if (currentNode.isWalkable)
            //    lastWalkableNode = currentNode;

            //move and aim to next node
            MoveAndAim(path[0].worldPosition);
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

    public override void OnExitTask()
    {
        base.OnExitTask();

        //remove path
        if (path != null)
            path.Clear();
    }

    #region private API

    void UpdatePath()
    {
        //delay between every update of the path
        if (Time.time > timerBeforeNextUpdatePath)
        {
            //reset timer
            timerBeforeNextUpdatePath = Time.time + delayRecalculatePath;

            //get path
            if (PathFindingAStar2D.instance && isPathProcessing == false)
            {
                isPathProcessing = true;
                PathFindingAStar2D.instance.FindPath(transformTask.position, target.position, OnFindPath, agentAStar);
            }
        }
    }

    void OnFindPath(List<Node2D> path)
    {
        //set path
        this.path = path;
        isPathProcessing = false;
    }

    void MoveAndAim(Vector3 destination)
    {
        //move to destination
        if(component)
            component.MoveInDirection((destination - transformTask.position).normalized, speedChase);

        //aim at destination
        if (aimComponent)
            aimComponent.AimAt(destination);
    }

    void CheckReachNode()
    {
        //if reach node, remove from list
        if (Vector2.Distance(transformTask.position, path[0].worldPosition) <= approxReachNode)
        {
            path.RemoveAt(0);
        }
    }

    #endregion
}
