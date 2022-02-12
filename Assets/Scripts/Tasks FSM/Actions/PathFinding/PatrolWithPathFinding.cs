using UnityEngine;
using redd096;
using redd096.PathFinding2D;

[AddComponentMenu("redd096/Tasks FSM/Action/PathFinding/Patrol With Path Finding")]
public class PatrolWithPathFinding : ActionTask
{
    [Header("Necessary Components - default get in parent")]
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
    [SerializeField] float delayRecalculatePath = 0.5f;

    float timerBeforeNextUpdatePath;
    Vector2 startPosition;
    float waitTimer;
    Path path;

    void OnDrawGizmos()
    {
        //draw radius patrol
        if (drawDebug)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : (Vector2)transformTask.position, radiusPatrol);
            Gizmos.color = Color.white;

            //draw path
            if (path != null && path.vectorPath != null && path.vectorPath.Count > 0)
            {
                Gizmos.color = Color.cyan;
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

        //save start position
        startPosition = transformTask.position;
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

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

        //if there is no path, find new one (at start or when reach destination)
        if (path == null || path.vectorPath == null || path.vectorPath.Count <= 0)
        {
            FindNewPath();
            return;
        }

        //wait when reached destination, before start moving again
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
        //delay between every update of the path (every few seconds, only if already calculated previous path)
        if (Time.time > timerBeforeNextUpdatePath && agentAStar && agentAStar.IsDone())
        {
            //reset timer
            timerBeforeNextUpdatePath = Time.time + delayRecalculatePath;

            //get random point in patrol area
            Vector3 randomPoint = startPosition + Random.insideUnitCircle * radiusPatrol;

            //get path
            agentAStar.FindPath(transformTask.position, randomPoint, OnPathComplete);
        }
    }

    void OnPathComplete(Path path)
    {
        //set path
        this.path = path;
    }

    void MoveAndAimToNextNode()
    {
        //move to node
        if(component)
            component.MoveTo(path.vectorPath[0], speedPatrol);

        //aim at next node of the path
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

    void CheckReachEndPath()
    {
        //when reach destination, set wait timer
        if (path == null || path.vectorPath == null || path.vectorPath.Count <= 0)
        {
            waitTimer = Time.time + timeToWaitWhenReach;
        }
    }

    #endregion
}
