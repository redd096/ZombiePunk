using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/PathFinding/Patrol With Path Finding")]
public class PatrolWithPathFinding : ActionTask
{
    [Header("Necessary Components - default get in parent or GameManager")]
    [SerializeField] MovementComponent component;
    [SerializeField] AimComponent aimComponent;
    [SerializeField] PathFindingAStar pathFinding;

    [Header("Patrol")]
    [SerializeField] float radiusPatrol = 5;
    [SerializeField] float speedPatrol = 2;
    [SerializeField] float timeToWaitWhenReach = 1;

    [Header("DEBUG")]
    [SerializeField] bool showDebug = false;
    [Range(0f, 0.5f)] [SerializeField] float approxReachNode = 0.05f;

    Vector2 startPosition;
    float waitTimer;
    List<Node> path;

    void OnDrawGizmos()
    {
        //draw radius patrol
        if (showDebug)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : (Vector2)transformTask.position, radiusPatrol);
            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetComponentInParent<MovementComponent>();
        if (aimComponent == null) aimComponent = GetComponentInParent<AimComponent>();
        if (pathFinding == null) pathFinding = GameManager.instance ? GameManager.instance.pathFindingAStar : null;

        //show warnings if not found
        if (GameManager.instance == null)
            Debug.LogWarning("Miss GameManager in scene");
        else if (pathFinding == null)
            Debug.LogWarning("Miss PathFinding in scene");

        //save start position
        startPosition = transformTask.position;
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        //if there is no path, find new one (at start or when reach destination)
        if (path == null || path.Count <= 0)
        {
            FindNewPath();
            return;
        }

        //wait
        if (waitTimer > Time.time)
        {
            return;
        }

        //move and aim
        MoveAndAimToNextNode();

        //when reach node, remove from list. If reach end of path, set wait timer
        CheckReachNode();
        CheckReachEndPath();
    }

    #region private API

    void FindNewPath()
    {
        //get random point in patrol area
        Vector3 randomPoint = startPosition + Random.insideUnitCircle * radiusPatrol;

        //get path
        if(pathFinding)
            path = pathFinding.FindPath(transformTask.position, randomPoint);
    }

    void MoveAndAimToNextNode()
    {
        //move to node
        if(component)
            component.MoveInDirection((path[0].worldPosition - transformTask.position).normalized, speedPatrol);

        //aim at next node of the path
        if (aimComponent)
            aimComponent.AimAt(path[0].worldPosition - transformTask.position);
    }

    void CheckReachNode()
    {
        //if reach node, remove from list
        if (Vector2.Distance(transformTask.position, path[0].worldPosition) <= approxReachNode)
        {
            path.RemoveAt(0);
        }
    }

    void CheckReachEndPath()
    {
        //when reach destination, set wait timer
        if (path == null || path.Count <= 0)
        {
            waitTimer = Time.time + timeToWaitWhenReach;
        }
    }

    #endregion
}
