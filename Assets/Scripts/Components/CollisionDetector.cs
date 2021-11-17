using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CollisionDetector : MonoBehaviour
{
	enum EUpdateModes { Update, FixedUpdate }
	public enum EDirectionEnum { up, right, left, down }

	[Header("Check Raycasts")]
	[Tooltip("Check collisions on Update or FixedUpdate?")] [SerializeField] EUpdateModes updateMode = EUpdateModes.Update;
	[Tooltip("Number of rays cast for every side horizontally")] [SerializeField] int numberOfHorizontalRays = 4;
	[Tooltip("Number of rays cast for every side vertically")] [SerializeField] int numberOfVerticalRays = 4;
	[Tooltip("A small value to accomodate for edge cases")] [SerializeField] float offsetRays = 0.1f;
	[Tooltip("Layers that raycasts ignore")] [SerializeField] LayerMask layersToIgnore = default;

	[Header("Necessary Components (by default get in children)")]
	[SerializeField] BoxCollider2D boxCollider = default;

	[Header("DEBUG")]
	[SerializeField] bool drawDebug = false;
	[ShowInInspector] bool IsHittingRight => rightHits.Count > 0;
	[ShowInInspector] bool IsHittingLeft => leftHits.Count > 0;
	[ShowInInspector] bool IsHittingUp => upHits.Count > 0;
	[ShowInInspector] bool IsHittingDown => downHits.Count > 0;

	//hits
	List<RaycastHit2D> rightHits = new List<RaycastHit2D>();
	List<RaycastHit2D> leftHits = new List<RaycastHit2D>();
	List<RaycastHit2D> upHits = new List<RaycastHit2D>();
	List<RaycastHit2D> downHits = new List<RaycastHit2D>();

	//bounds
	Vector2 centerBounds;
	float upBounds;
	float downBounds;
	float rightBounds;
	float leftBounds;

	//for debug
	float debugDrawDuration = -1;

	void Update()
	{
		//do only if update mode is Update
		if (updateMode == EUpdateModes.Update)
			UpdateCollisions();
	}

	void FixedUpdate()
	{
		//do only if update mode is FixedUpdate
		if (updateMode == EUpdateModes.FixedUpdate)
			UpdateCollisions();
	}

	[Button()]
	void DrawCollisions()
	{
		//be sure drawDebug is true
		bool previousDraw = drawDebug;
		drawDebug = true;

		//set time
		debugDrawDuration = 2;

		//check collisions
		UpdateCollisions();

		//restore debug values
		drawDebug = previousDraw;
		debugDrawDuration = -1;
	}

	#region private API

	bool CheckComponents()
	{
		//be sure to have a box collider
		if (boxCollider == null)
		{
			boxCollider = GetComponentInChildren<BoxCollider2D>();

			if (boxCollider == null)
			{
				Debug.LogWarning("Miss BoxCollider on " + name);
				return false;
			}
		}

		return true;
	}

	void UpdateVars()
	{
		//update bounds
		centerBounds = boxCollider.bounds.center;
		upBounds = boxCollider.bounds.center.y + boxCollider.bounds.extents.y;
		downBounds = boxCollider.bounds.center.y - boxCollider.bounds.extents.y;
		rightBounds = boxCollider.bounds.center.x + boxCollider.bounds.extents.x;
		leftBounds = boxCollider.bounds.center.x - boxCollider.bounds.extents.x;
	}

	void CheckCollisionsHorizontal()
	{
		//horizontal raycast vars
		Vector2 raycastOriginBottom = new Vector2(centerBounds.x, downBounds + offsetRays);
		Vector2 raycastOriginTop = new Vector2(centerBounds.x, upBounds - offsetRays);
		float raycastHorizontalLength = (rightBounds - leftBounds) * 0.5f;
		rightHits.Clear();
		leftHits.Clear();

		for (int i = 0; i < numberOfHorizontalRays; i++)
		{
			//from bottom to top
			Vector2 raycastOriginPoint = Vector2.Lerp(raycastOriginBottom, raycastOriginTop, (float)i / (numberOfHorizontalRays - 1));

			//raycast right and left
			RaycastHit2D rightHit = RayCastHitSomething(raycastOriginPoint, Vector2.right, raycastHorizontalLength);
			RaycastHit2D leftHit = RayCastHitSomething(raycastOriginPoint, Vector2.left, raycastHorizontalLength);

			//save hits
			if (rightHit) rightHits.Add(rightHit);
			if (leftHit) leftHits.Add(leftHit);

			//debug raycasts
			if (drawDebug)
			{
				DebugRaycast(raycastOriginPoint, Vector2.right, raycastHorizontalLength, rightHit ? Color.red : Color.cyan);
				DebugRaycast(raycastOriginPoint, Vector2.left, raycastHorizontalLength, leftHit ? Color.red : Color.cyan);
			}
		}
	}

	void CheckCollisionsVertical()
	{
		//vertical raycast vars
		Vector2 raycastOriginLeft = new Vector2(leftBounds + offsetRays, centerBounds.y);
		Vector2 raycastOriginRight = new Vector2(rightBounds - offsetRays, centerBounds.y);
		float raycastVerticalLength = (upBounds - downBounds) * 0.5f;
		upHits.Clear();
		downHits.Clear();

		for (int i = 0; i < numberOfVerticalRays; i++)
		{
			//from left to right
			Vector2 raycastOriginPoint = Vector2.Lerp(raycastOriginLeft, raycastOriginRight, (float)i / (numberOfVerticalRays - 1));

			//raycasts up and down
			RaycastHit2D upHit = RayCastHitSomething(raycastOriginPoint, Vector2.up, raycastVerticalLength);
			RaycastHit2D downHit = RayCastHitSomething(raycastOriginPoint, Vector2.down, raycastVerticalLength);

			//save hits
			if (upHit) upHits.Add(upHit);
			if (downHit) downHits.Add(downHit);

			//debug raycasts
			if (drawDebug)
			{
				DebugRaycast(raycastOriginPoint, Vector2.up, raycastVerticalLength, upHit ? Color.red : Color.blue);
				DebugRaycast(raycastOriginPoint, Vector2.down, raycastVerticalLength, downHit ? Color.red : Color.blue);
			}
		}
	}

	RaycastHit2D RayCastHitSomething(Vector2 originPoint, Vector2 direction, float distance)
	{
		float distanceToNearest = Mathf.Infinity;
		RaycastHit2D nearest = default;

		foreach (RaycastHit2D hit in Physics2D.RaycastAll(originPoint, direction, distance, ~layersToIgnore))
		{
			//for every hit, be sure to not hit self
			if (hit && hit.collider != boxCollider)
			{
				//calculate nearest hit
				if (hit.distance < distanceToNearest)
				{
					distanceToNearest = hit.distance;
					nearest = hit;
				}
			}
		}

		return nearest;
	}

	void DebugRaycast(Vector2 originPoint, Vector2 direction, float distance, Color color)
	{
		//debug
		if (debugDrawDuration > 0)
			Debug.DrawRay(originPoint, direction * distance, color, debugDrawDuration);     //when called by press the button, visualizare for few seconds
		else
			Debug.DrawRay(originPoint, direction * distance, color);                        //else show at every update
	}

	#endregion

	#region public API

	/// <summary>
	/// Update collisions
	/// </summary>
	public void UpdateCollisions()
	{
		//start only if there are all necessary components
		if (CheckComponents() == false)
			return;

		//check collisions
		UpdateVars();
		CheckCollisionsHorizontal();
		CheckCollisionsVertical();
	}

	/// <summary>
	/// Return last hitting information in direction (call UpdateCollisions to have updated informations)
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public bool IsHitting(EDirectionEnum direction)
	{
		switch (direction)
		{
			case EDirectionEnum.up:
				return upHits.Count > 0;
			case EDirectionEnum.right:
				return rightHits.Count > 0;
			case EDirectionEnum.left:
				return leftHits.Count > 0;
			case EDirectionEnum.down:
				return downHits.Count > 0;
			default:
				return false;
		}
	}

	/// <summary>
	/// Return last hits in direction (call UpdateCollisions to have updated informations)
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public RaycastHit2D[] GetHits(EDirectionEnum direction)
	{
		switch (direction)
		{
			case EDirectionEnum.up:
				return upHits.ToArray();
			case EDirectionEnum.right:
				return rightHits.ToArray();
			case EDirectionEnum.left:
				return leftHits.ToArray();
			case EDirectionEnum.down:
				return downHits.ToArray();
			default:
				return null;
		}
	}

	/// <summary>
	/// Return updated center of the bounds
	/// </summary>
	/// <returns></returns>
	public Vector2 GetCenterBound()
	{
		UpdateVars();
		return centerBounds;
	}

	/// <summary>
	/// Return updated bounds informations
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public float GetBounds(EDirectionEnum direction)
	{
		//update vars
		UpdateVars();

		switch (direction)
		{
			case EDirectionEnum.up:
				return upBounds;
			case EDirectionEnum.right:
				return rightBounds;
			case EDirectionEnum.left:
				return leftBounds;
			case EDirectionEnum.down:
				return downBounds;
			default:
				return 0;
		}
	}

	#endregion
}
