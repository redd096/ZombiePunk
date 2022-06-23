using UnityEngine;
using redd096;
using redd096.Attributes;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Aim/Aim At Position")]
public class AimAtPosition : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] AimComponent component;

    [Header("Aim")]
    [SerializeField] bool getPositionFromTask = true;
    [EnableIf("getPositionFromTask")] [SerializeField] SimpleMoveToPosition taskWithPosition = default;
    [DisableIf("getPositionFromTask")] [SerializeField] Vector2 positionToAim = Vector2.zero;
    [Tooltip("Rotate immediatly or use a rotation speed")] [SerializeField] bool rotateUsingSpeed = false;
    [EnableIf("rotateUsingSpeed")] [SerializeField] float rotationSpeed = 50;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    //events
    public System.Action onStartAimAtTarget { get; set; }
    public System.Action onEndAimAtTarget { get; set; }

    Vector2 position;

    void OnDrawGizmos()
    {
        //draw line to reach position and to target
        if (drawDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transformTask.position, Application.isPlaying && component ? (Vector2)transformTask.position + component.AimDirectionInput * 2 : (Vector2)transformTask.position + Vector2.right * 2);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transformTask.position, getPositionFromTask && taskWithPosition ? taskWithPosition.PositionToReach : positionToAim);
            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetStateMachineComponent<AimComponent>();
        if (taskWithPosition == null) taskWithPosition = GetComponent<SimpleMoveToPosition>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        position = getPositionFromTask && taskWithPosition ? taskWithPosition.PositionToReach : positionToAim;

        //call event
        onStartAimAtTarget?.Invoke();
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //call event
        onEndAimAtTarget?.Invoke();
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        //aim at target
        if (component)
        {
            //immediatly
            if (rotateUsingSpeed == false)
            {
                component.AimAt(position);
            }
            //or with rotation speed
            else
            {
                //calculate direction to target
                Vector2 directionToReach = (position - (Vector2)transformTask.position).normalized;     //direction to target
                float angle = Vector2.SignedAngle(component.AimDirectionInput, directionToReach);       //rotation angle

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
