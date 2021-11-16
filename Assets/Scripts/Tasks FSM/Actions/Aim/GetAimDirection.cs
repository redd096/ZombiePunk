using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/Aim")]
[Description("Get Aim Direction from AimComponent and save in the blackboard")]
public class GetAimDirection : ActionTask<AimComponent>
{
	[BlackboardOnly] public BBParameter<Vector2> saveAs;
	public bool repeat;

	protected override void OnExecute() { Do(); }   //same as OnEnable
	protected override void OnUpdate() { Do(); }

	void Do()
    {
		//save aim direction input
		saveAs.value = agent.AimDirectionInput;

		//end action if necessary
		if (!repeat) { EndAction(); }
	}
}
