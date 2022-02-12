using System.Collections.Generic;
using UnityEngine;
using redd096;
using redd096.PathFinding2D;

[AddComponentMenu("redd096/Tasks FSM/Action/PathFinding/Patrol With Path Finding")]
public class PatrolWithPathFinding : ActionTask
{
    [Header("Necessary Components - default get in parent or GameManager")]
    [SerializeField] MovementComponent component;
    [SerializeField] AimComponent aimComponent;
    [SerializeField] AgentAStar2D agentAStar;

    [Header("Patrol")]
    [SerializeField] float radiusPatrol = 5;
    [SerializeField] float speedPatrol = 2;
    [SerializeField] float timeToWaitWhenReach = 1;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;
    [Range(0f, 0.5f)] [SerializeField] float approxReachNode = 0.05f;
    [SerializeField] float delayRecalculatePath = 0.2f;

    float timerBeforeNextUpdatePath;
    Vector2 startPosition;
    float waitTimer;
    List<Node2D> path;
    bool isProcessingPath;

    void OnDrawGizmos()
    {
        //draw radius patrol
        if (drawDebug)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : (Vector2)transformTask.position, radiusPatrol);
            Gizmos.color = Color.white;

            //draw path
            if (path != null && path.Count > 0)
            {
                Gizmos.color = Color.cyan;
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

        //save start position
        startPosition = transformTask.position;
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //remove previous path
        if (path != null)
            path.Clear();
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
        //delay between every update of the path
        if (Time.time > timerBeforeNextUpdatePath)
        {
            //reset timer
            timerBeforeNextUpdatePath = Time.time + delayRecalculatePath;

            //get random point in patrol area
            Vector3 randomPoint = startPosition + Random.insideUnitCircle * radiusPatrol;

            //get path
            if (PathFindingAStar2D.instance && isProcessingPath == false)
            {
                isProcessingPath = true;
                PathFindingAStar2D.instance.FindPath(transformTask.position, randomPoint, OnFindPath, agentAStar);
            }
        }
    }

    void OnFindPath(List<Node2D> path)
    {
        //set path
        this.path = path;
        isProcessingPath = false;
    }

    void MoveAndAimToNextNode()
    {
        //move to node
        if(component)
            component.MoveInDirection((path[0].worldPosition - (Vector2)transformTask.position).normalized, speedPatrol);

        //aim at next node of the path
        if (aimComponent)
            aimComponent.AimAt(path[0].worldPosition);
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
