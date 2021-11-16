using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[Category("redd096/New Input System")]
[Description("Get Button value by string name. Get also if was pressed this frame")]
#if ENABLE_INPUT_SYSTEM
public class GetNewInputButton : ActionTask<PlayerInput>
#else
public class GetNewInputButton : ActionTask
#endif
{
	public BBParameter<string> buttonName = "Interact";
	[BlackboardOnly] public BBParameter<bool> saveAs;
	[BlackboardOnly] public BBParameter<bool> savePressedThisFrameAs;
	public bool repeat;

	protected override void OnExecute() { Do(); }   //same as OnEnable
	protected override void OnUpdate() { Do(); }

	void Do()
	{
#if ENABLE_INPUT_SYSTEM
		//save vars
		saveAs.value = string.IsNullOrEmpty(buttonName.value) ? false : agent.actions.FindAction(buttonName.value).phase == InputActionPhase.Started;
		savePressedThisFrameAs.value = string.IsNullOrEmpty(buttonName.value) ? false : agent.actions.FindAction(buttonName.value).triggered;
#endif

		//end action if necessary
		if (!repeat) { EndAction(); }
	}
}
