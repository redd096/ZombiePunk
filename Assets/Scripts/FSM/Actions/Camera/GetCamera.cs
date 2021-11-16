using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("redd096/Camera")]
[Description("Get Camera and save in the blackboard")]
public class GetCamera : ActionTask
{
	public BBParameter<bool> getOnlyIfNull = true;
	[BlackboardOnly] public BBParameter<Camera> saveAs;

	[ParadoxNotion.Design.Header("Possible ways to find")]
	public BBParameter<bool> getMainCamera = true;
	public BBParameter<bool> getByName = true;
	public BBParameter<string> cameraName = "MainCamera";
	public BBParameter<bool> findObjectOfType = true;

	[ParadoxNotion.Design.Header("Repeat")]
	public bool repeat;

	protected override void OnExecute() { Do(); }   //same as OnEnable
	protected override void OnUpdate() { Do(); }

	void Do()
	{
		//get if camera is null or always if necessary
		if(saveAs.value == null || getOnlyIfNull.value == false)
        {
			saveAs.value = FindCamera();
        }

		//end action if necessary
		if (!repeat) { EndAction(); }
	}

	Camera FindCamera()
	{
		Camera cam = null;

		//get main camera
		if (getMainCamera.value)
		{
			cam = Camera.main;
			if (cam)
				return cam;
		}

		//or get by name
		if (getByName.value)
		{
			cam = GameObject.Find(cameraName.value).GetComponent<Camera>();
			if (cam)
				return cam;
		}

		//or get by type
		if (findObjectOfType.value)
		{
			cam = Object.FindObjectOfType<Camera>();
			if (cam)
				return cam;
		}

		return cam;
	}
}
