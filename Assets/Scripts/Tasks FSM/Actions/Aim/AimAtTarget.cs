using UnityEngine;
using redd096;
using NaughtyAttributes;

[AddComponentMenu("redd096/Tasks FSM/Action/Aim/Aim At Target")]
public class AimAtTarget : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] AimComponent component;

    [Header("Aim")]
    [SerializeField] string targetBlackboardName = "Target";
    [Tooltip("Rotate immediatly or use a rotation speed")] [SerializeField] bool rotateUsingSpeed = false;
    [EnableIf("rotateUsingSpeed")] [SerializeField] float rotationSpeed = 50;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    Transform target;

    void OnDrawGizmos()
    {
        //draw line to reach position and to target
        if (drawDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transformTask.position, Application.isPlaying && component ? (Vector2)transformTask.position + component.AimDirectionInput * 2 : (Vector2)transformTask.position + Vector2.right * 2);
            Gizmos.color = Color.cyan;
            if (target) Gizmos.DrawLine(transformTask.position, target.position);
            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetStateMachineComponent<AimComponent>();
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

        //aim at target
        if (component)
        {
            //immediatly
            if (rotateUsingSpeed == false)
            {
                component.AimAt(target.position);
            }
            //or with rotation speed
            else
            {
                //calculate direction to target
                Vector2 directionToReach = (target.position - transformTask.position).normalized;   //direction to target
                float angle = Vector2.SignedAngle(component.AimDirectionInput, directionToReach);   //rotation angle

                //rotate only if not already looking at target
                if (Mathf.Abs(angle) > Mathf.Epsilon)
                {
                    //calculate rotation, but if exceed, clamp it
                    float rotationAngle = rotationSpeed * Time.deltaTime > Mathf.Abs(angle) ? angle : rotationSpeed * Time.deltaTime * Mathf.Sign(angle);
                    Vector2 newAimPosition = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * component.AimDirectionInput;

                    //set new aim position
                    component.AimAt((Vector2)transformTask.position + newAimPosition);
                }
            }
        }
    }
}
