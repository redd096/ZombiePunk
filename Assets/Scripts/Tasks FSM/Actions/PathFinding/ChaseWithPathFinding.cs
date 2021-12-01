using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/PathFinding/Chase With Path Finding")]
public class ChaseWithPathFinding : ActionTask
{
    [Header("Necessary Components - default get in parent or GameManager")]
    [SerializeField] MovementComponent component;
    [SerializeField] AimComponent aimComponent;
    [SerializeField] PathFindingAStar pathFinding;

    [Header("Chase")]
    [SerializeField] string targetBlackboardName = "Target";
    [SerializeField] float speedChase = 5;

    [Header("DEBUG")]
    [Range(0f, 0.5f)] [SerializeField] float approxReachNode = 0.05f;

    Transform target;
    List<Node> path;
    Node lastWalkableNode;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetStateMachineComponent<MovementComponent>();
        if (aimComponent == null) aimComponent = GetStateMachineComponent<AimComponent>();
        if (pathFinding == null) pathFinding = GameManager.instance ? GameManager.instance.pathFindingAStar : null;

        //show warnings if not found
        if (GameManager.instance == null)
            Debug.LogWarning("Miss GameManager in scene");
        else if (pathFinding == null)
            Debug.LogWarning("Miss PathFinding in scene");
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //get target from blackboard
        target = stateMachine.GetBlackboardElement(targetBlackboardName) as Transform;
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
            //if on a walkable node, save it
            Node currentNode = pathFinding.Grid.NodeFromWorldPosition(transformTask.position);
            if (currentNode.isWalkable)
                lastWalkableNode = currentNode;

            //move and aim to next node
            MoveAndAim(path[0].worldPosition);
            CheckReachNode();
        }
        //if there is no path, move straight to target (only if last walkable node is setted)
        else if(lastWalkableNode != null)
        {
            //if target is in a not walkable node, but neighbour of our last walkable node, move straight to it
            Node targetNode = pathFinding.Grid.NodeFromWorldPosition(target.position);
            if (pathFinding.Grid.GetNeighbours(lastWalkableNode).Contains(targetNode))
            {
                MoveAndAim(target.position);
            }
            //else move back to last walkable node
            else
            {
                MoveAndAim(lastWalkableNode.worldPosition);
            }
        }
    }

    #region private API

    void UpdatePath()
    {
        //get path
        path = pathFinding.FindPath(transformTask.position, target.position);
    }

    void MoveAndAim(Vector3 destination)
    {
        //move to destination
        if(component)
            component.MoveInDirection((destination - transformTask.position).normalized, speedChase);

        //aim at destination
        if (aimComponent)
            aimComponent.AimAt(destination - transformTask.position);
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
