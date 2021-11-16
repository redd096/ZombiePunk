using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[Category("redd096/New Input System")]
[Description("Get Vector2 value by string name, you can also normalize it to get a Vector2 of magnitude 1")]
#if ENABLE_INPUT_SYSTEM
public class GetNewInputVector2 : ActionTask<PlayerInput>
#else
public class GetNewInputVector2 : ActionTask
#endif
{
	public BBParameter<string> vector2Name = "Move";
	public BBParameter<bool> normalize = true;
	[BlackboardOnly] public BBParameter<Vector2> saveAs;
	[BlackboardOnly] public BBParameter<float> saveXAs;
	[BlackboardOnly] public BBParameter<float> saveYAs;
	public bool repeat;

	protected override void OnExecute() { Do(); }	//same as OnEnable
	protected override void OnUpdate() { Do(); }

	void Do()
	{
#if ENABLE_INPUT_SYSTEM
		//get vector2 - normalize it if necessary
		var vector2 = string.IsNullOrEmpty(vector2Name.value) ? Vector2.zero : agent.actions.FindAction(vector2Name.value).ReadValue<Vector2>();
		if (normalize.value)
			vector2 = vector2.normalized;

		//save vars
		saveAs.value = vector2;
		saveXAs.value = vector2.x;
		saveYAs.value = vector2.y;
#endif

		//end action if necessary
		if (!repeat) { EndAction(); }
	}
}
