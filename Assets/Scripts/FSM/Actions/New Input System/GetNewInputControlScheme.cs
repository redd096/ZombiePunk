using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[Category("redd096/New Input System")]
[Description("Get current control scheme in PlayerInput")]
#if ENABLE_INPUT_SYSTEM
public class GetNewInputControlScheme : ActionTask<PlayerInput>
#else
public class GetNewInputControlScheme : ActionTask
#endif
{
	[BlackboardOnly] public BBParameter<string> saveAs;
    public bool repeat;

    protected override void OnExecute() { Do(); }   //same as OnEnable
    protected override void OnUpdate() { Do(); }

	void Do()
	{
#if ENABLE_INPUT_SYSTEM
		//save current control scheme
		saveAs.value = agent.currentControlScheme;
#endif

		//end action if necessary
		if (!repeat) { EndAction(); }
	}
}
