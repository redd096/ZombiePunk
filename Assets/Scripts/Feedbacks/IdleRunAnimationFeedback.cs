using UnityEngine;
using NaughtyAttributes;

namespace redd096
{
    [AddComponentMenu("redd096/Feedbacks/Idle Run Animation Feedback")]
    public class IdleRunAnimationFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in child and parent")]
        [SerializeField] Animator anim = default;
        [SerializeField] MovementComponent movementComponent = default;

        [Header("Run when speed > this value")]
        [SerializeField] float valueToRun = 0.1f;

        [Header("Animations - play by state name or set bool parameter")]
        [SerializeField] bool playStateName = true;
        [EnableIf("playStateName")] [SerializeField] string idleAnimation = "Idle";
        [EnableIf("playStateName")] [SerializeField] string runAnimation = "Run";
        [EnableIf("setBoolParameter")] [SerializeField] string boolName = "IsRunning";
        bool setBoolParameter => !playStateName;

        bool isRunning;

        void OnEnable()
        {
            if (anim == null) anim = GetComponentInChildren<Animator>();
            if (movementComponent == null) movementComponent = GetComponentInParent<MovementComponent>();
        }

        void Update()
        {
            if(anim && movementComponent)
            {
                //start run
                if(movementComponent.CurrentSpeed > valueToRun && isRunning == false)
                {
                    isRunning = true;

                    //set animator
                    if (playStateName)
                        anim.Play(runAnimation);
                    else
                        anim.SetBool(boolName, true);
                }
                //back to idle
                else if(movementComponent.CurrentSpeed <= valueToRun && isRunning)
                {
                    isRunning = false;

                    //set animator
                    if (playStateName)
                        anim.Play(idleAnimation);
                    else
                        anim.SetBool(boolName, false);
                }
            }
        }
    }
}