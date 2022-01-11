using System.Collections.Generic;
using UnityEngine;
using redd096;
using NaughtyAttributes;

[AddComponentMenu("redd096/Tasks FSM/Condition/Check Can See Target 2D Advanced")]
public class CheckCanSeeTarget2DAdvanced : ConditionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] AimComponent component;

    [Header("Can See Component 2D")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float maxDistance = 5;
    [SerializeField] float awarnessDistance = 1f;
    [Tooltip("Look only left and right, or use AimComponent direction")] [SerializeField] bool useOnlyLeftAndRight = false;
    [Range(1, 180)] public float viewAngle = 70f;
    [Tooltip("Check if there is a wall between this object and target")] [SerializeField] bool checkViewClear = true;
    [EnableIf("checkViewClear")] [SerializeField] LayerMask layerWalls;
    [SerializeField] string saveTargetInBlackboardAs = "Target";

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    List<Transform> possibleTargets = new List<Transform>();

    void OnDrawGizmos()
    {
        if(drawDebug)
        {
            Gizmos.color = Color.red;

            //draw max distance
            Gizmos.DrawWireSphere(transformTask.position, maxDistance);

            //draw awareness
            Gizmos.DrawWireSphere(transformTask.position, awarnessDistance);

            //draw view angle
            Vector2 direction = Application.isPlaying && component ? (useOnlyLeftAndRight ? (component.IsLookingRight ? Vector2.right : Vector2.left) : component.AimDirectionInput) : Vector2.right;            
            Gizmos.matrix = Matrix4x4.TRS(transformTask.position, Quaternion.LookRotation(direction), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, viewAngle, maxDistance, 0, 1f);

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if(component == null) component = GetStateMachineComponent<AimComponent>();
    }

    public override bool OnCheckTask()
    {
        //find every targets inside max distance
        FindPossibleTargets();

        foreach (Transform t in new List<Transform>(possibleTargets))
        {
            //remove targets if can't see them
            if ((IsInsideAwareness(t) == false && IsInsideView(t) == false)     //if not inside awareness distance and not inside view angle
                || IsViewClear(t) == false)                                     //or view is not clear
            {
                possibleTargets.Remove(t);
            }
        }

        if (possibleTargets.Count <= 0)
            return false;

        //if found targets, save nearest in the blackboard
        stateMachine.SetBlackboardElement(saveTargetInBlackboardAs, GetNearest());

        return true;
    }

    #region private API

    void FindPossibleTargets()
    {
        //clear list
        possibleTargets.Clear();

        //find every element in max distance, using layer
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transformTask.position, maxDistance, targetLayer.value))
        {
            //get component, if setted
            //if (string.IsNullOrEmpty(componentToFind) == false)
            //{
            //    Component c = col.GetComponentInParent(System.Type.GetType(componentToFind));
            //    if (c)
            //    {
            //        possibleTargets.Add(c.transform);
            //    }
            //
            //    continue;
            //}

            //else add without check component
            possibleTargets.Add(col.transform);
        }
    }

    bool IsInsideAwareness(Transform t)
    {
        //check is inside awareness distance
        return Vector2.Distance(transformTask.position, t.position) < awarnessDistance;
    }

    bool IsInsideView(Transform t)
    {
        if (component == null)
            return false;

        //check is inside view angle (use IsLookingRight to check right or left, else use aim direction input)
        return Vector2.Angle(t.position - transformTask.position, useOnlyLeftAndRight ? (component.IsLookingRight ? Vector2.right : Vector2.left) : component.AimDirectionInput) < viewAngle;
    }

    bool IsViewClear(Transform t)
    {
        //check there is nothing between
        return checkViewClear == false || Physics2D.Linecast(transformTask.position, t.position, layerWalls.value) == false;
    }

    Transform GetNearest()
    {
        float distance = Mathf.Infinity;
        Transform nearest = null;

        //find nearest
        foreach (Transform t in possibleTargets)
        {
            if (Vector2.Distance(transformTask.position, t.position) < distance)
            {
                distance = Vector2.Distance(transformTask.position, t.position);
                nearest = t;
            }
        }

        return nearest;
    }

    #endregion
}
