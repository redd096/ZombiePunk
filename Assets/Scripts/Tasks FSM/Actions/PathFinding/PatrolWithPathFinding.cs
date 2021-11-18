using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/PathFinding")]
[Description("Patrol inside a radius using MovementComponent and PathFinding")]
public class PatrolWithPathFinding : ActionTask<MovementComponent>
{
    public BBParameter<float> radiusPatrol = 5;
    public BBParameter<float> speedPatrol = 5;
    public BBParameter<float> approxReachNode = 0.05f;
    public BBParameter<float> timeToWaitWhenReach = 5;
    [BlackboardOnly] public BBParameter<List<redd096.Node>> savePathAs;
    public bool repeat;

    Vector2 startPosition;
    float waitTimer;

    public override void OnDrawGizmosSelected()
    {
        //draw radius patrol
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : (Vector2)agent.transform.position, radiusPatrol.value);
        Gizmos.color = Color.white;
    }

    protected override string OnInit()
    {
        //save start position
        startPosition = agent.transform.position;
        return base.OnInit();
    }

    //on enable
    protected override void OnExecute()
    {
        //if there is no path, find new one (in execute, so EndAction will be called when reach destination, after wait timer)
        if (savePathAs.value == null || savePathAs.value.Count <= 0)
        {
            FindNewPath();
        }
    }

    protected override void OnUpdate()
    {
        //wait
        if (waitTimer > Time.time)
        {
            return;
        }

        //if there is no path, find new one (when reach destination)
        if(savePathAs.value == null || savePathAs.value.Count <= 0)
        {
            FindNewPath();

            //end action if necessary (because if is here, it has already reached path setted on execute and waited timer)
            if (!repeat) { EndAction(); }
            return;
        }

        //pass direction and speed to MovementComponent
        agent.MoveInDirection((savePathAs.value[0].worldPosition - agent.transform.position).normalized, speedPatrol.value);

        //when reach node, remove from list. If reach end of path, set wait timer
        CheckReachNode();
        CheckReachEndPath();
    }

    #region private API

    void FindNewPath()
    {
        //get random point in patrol area
        Vector3 randomPoint = startPosition + Random.insideUnitCircle * radiusPatrol.value;

        //get path
        savePathAs.value = GameManager.instance.pathFindingAStar.FindPath(agent.transform.position, randomPoint);
    }

    void CheckReachNode()
    {
        //if reach node, remove from list
        if(Vector2.Distance(agent.transform.position, savePathAs.value[0].worldPosition) <= approxReachNode.value)
        {
            savePathAs.value.RemoveAt(0);
        }
    }

    void CheckReachEndPath()
    {
        //when reach destination, set wait timer
        if (savePathAs.value == null || savePathAs.value.Count <= 0)
        {
            waitTimer = Time.time + timeToWaitWhenReach.value;
        }
    }

    #endregion
}
