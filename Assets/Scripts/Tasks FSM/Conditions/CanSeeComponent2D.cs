using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096")]
[Description("Find target by component, using a combination of line of sight and view angle check")]
public class CanSeeComponent2D : ConditionTask<AimComponent>
{
    public BBParameter<LayerMask> targetLayer;
    public BBParameter<string> componentToFind;
    public BBParameter<float> maxDistance = 5;
    public BBParameter<float> awarnessDistance = 1f;
    [SliderField(1, 180)]
    public BBParameter<float> viewAngle = 70f;
    public BBParameter<LayerMask> layerWalls;
    [BlackboardOnly] public BBParameter<Transform> saveTargetAs;

    List<Transform> possibleTargets = new List<Transform>();

    public override void OnDrawGizmosSelected()
    {
        if (agent != null)
        {
            //draw max distance
            Gizmos.DrawWireSphere(agent.transform.position, maxDistance.value);

            //draw awareness
            Gizmos.DrawWireSphere(agent.transform.position, awarnessDistance.value);

            //draw view angle
            Vector2 direction = Application.isPlaying ? agent.AimDirectionInput : Vector2.right;
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, Quaternion.LookRotation(direction), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, viewAngle.value, maxDistance.value, 0, 1f);
        }
    }

    protected override bool OnCheck()
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

        //if found targets, save nearest
        saveTargetAs.value = GetNearest();

        return true;
    }

    #region private API

    void FindPossibleTargets()
    {
        //clear list
        possibleTargets.Clear();

        //find every element in max distance, using layer
        foreach (Collider2D col in Physics2D.OverlapCircleAll(agent.transform.position, maxDistance.value, targetLayer.value))
        {
            //get component, if setted
            if (string.IsNullOrEmpty(componentToFind.value) == false)
            {
                Component component = col.GetComponentInParent(System.Type.GetType(componentToFind.value));
                if (component)
                {
                    possibleTargets.Add(component.transform);
                }

                continue;
            }

            //else add without check component
            possibleTargets.Add(col.transform);
        }
    }

    bool IsInsideAwareness(Transform t)
    {
        //check is inside awareness distance
        return Vector2.Distance(agent.transform.position, t.position) < awarnessDistance.value;
    }

    bool IsInsideView(Transform t)
    {
        //check is inside view angle (use IsLookingRight to check right or left)
        return Vector2.Angle(t.position - agent.transform.position, agent.AimDirectionInput) < viewAngle.value;
    }

    bool IsViewClear(Transform t)
    {
        //check there is nothing between
        return Physics2D.Linecast(agent.transform.position, t.position, layerWalls.value) == false;
    }

    Transform GetNearest()
    {
        float distance = Mathf.Infinity;
        Transform nearest = null;

        //find nearest
        foreach (Transform t in possibleTargets)
        {
            if (Vector2.Distance(agent.transform.position, t.position) < distance)
            {
                distance = Vector2.Distance(agent.transform.position, t.position);
                nearest = t;
            }
        }

        return nearest;
    }

    #endregion
}
