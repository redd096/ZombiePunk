using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class InteractComponent : MonoBehaviour
{
	enum EUpdateModes { Update, FixedUpdate, Coroutine }

	[Header("Find Interactables")]
	[Tooltip("Check collisions on Update or FixedUpdate?")] [SerializeField] EUpdateModes updateMode = EUpdateModes.Coroutine;
	[EnableIf("updateMode", EUpdateModes.Coroutine)] [SerializeField] float timeCoroutine = 0.1f;

	//update mode
	Coroutine updateCoroutine;

	void OnEnable()
	{
		//start coroutine
		if (updateMode == EUpdateModes.Coroutine)
			updateCoroutine = StartCoroutine(UpdateCoroutine());
	}

	void OnDisable()
	{
		//be sure to stop coroutine
		if (updateCoroutine != null)
		{
			StopCoroutine(updateCoroutine);
			updateCoroutine = null;
		}
	}

	void Update()
	{
		//do only if update mode is Update
		if (updateMode == EUpdateModes.Update)
			FindInteractables();
	}

	void FixedUpdate()
	{
		//do only if update mode is FixedUpdate
		if (updateMode == EUpdateModes.FixedUpdate)
			FindInteractables();
	}

	IEnumerator UpdateCoroutine()
	{
		//do only if update mode is Coroutine
		while (updateMode == EUpdateModes.Coroutine)
		{
			FindInteractables();
			yield return new WaitForSeconds(timeCoroutine);
		}
	}

    #region public API

	/// <summary>
	/// Find every interactable in area
	/// </summary>
    public void FindInteractables()
    {

    }

    #endregion
}
