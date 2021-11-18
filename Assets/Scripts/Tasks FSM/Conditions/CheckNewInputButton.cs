using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[Category("redd096")]
[Description("Check if button is pressed")]
#if ENABLE_INPUT_SYSTEM
public class CheckNewInputButton : ConditionTask<PlayerInput>
#else
public class CheckNewInputButton : ConditionTask
#endif
{
    public enum EPressType { Pressed, Down }

    public EPressType pressType = EPressType.Down;
    [RequiredField] public BBParameter<string> buttonName = "Pause";

    protected override bool OnCheck()
    {
#if ENABLE_INPUT_SYSTEM
        if (pressType == EPressType.Pressed)
            return agent.actions.FindAction(buttonName.value).phase == InputActionPhase.Started;
        else if(pressType == EPressType.Down)
            return agent.actions.FindAction(buttonName.value).triggered;
#endif

        return false;
    }
}
