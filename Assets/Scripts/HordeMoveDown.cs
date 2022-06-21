using System.Collections;
using UnityEngine;
using redd096.Attributes;

public class HordeMoveDown : MonoBehaviour
{
    enum EAxis { X, Y }

    [Header("If object not setted, find by name")]
    [SerializeField] GameObject hordeObjectInScene = default;
    [SerializeField] string hordeName = "HORDE";

    [Header("Move down in time")]
    [SerializeField] EAxis movementAxis = EAxis.Y;
    [SerializeField] bool useAnimationCurve = false;
    [EnableIf("useAnimationCurve")] [SerializeField] AnimationCurve animationCurve = default;
    [DisableIf("useAnimationCurve")] [SerializeField] float duration = 1;
    [DisableIf("useAnimationCurve")] [SerializeField] float moveOf = -3;

    Coroutine moveDownCoroutine;

    void Start()
    {
        //if null, find by name
        if (hordeObjectInScene == null)
            hordeObjectInScene = GameObject.Find(hordeName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hordeObjectInScene == null)
            return;

        //check if collision or some object in it, is our horde
        foreach (Transform child in collision.transform.root.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject == hordeObjectInScene)
            {
                //start coroutine
                if (moveDownCoroutine == null)
                    moveDownCoroutine = StartCoroutine(MoveDownCoroutine());
            }
        }
    }

    IEnumerator MoveDownCoroutine()
    {
        //save start vars
        float startPosition = GetPositionAxis( hordeObjectInScene ? (Vector2)hordeObjectInScene.transform.position : Vector2.zero );
        float startTime = Time.time;
        float delta = 0;

        while (true)
        {
            //be sure horde is still in scene
            if (hordeObjectInScene == null)
                break;

            float pos;

            //set position using animation curve
            if (useAnimationCurve)
            {
                pos = startPosition + animationCurve.Evaluate(Time.time - startTime);
            }
            //or lerp
            else
            {
                delta += Time.deltaTime / duration;
                pos = Mathf.Lerp(startPosition, startPosition + moveOf, delta);
            }

            //set new position
            hordeObjectInScene.transform.position = GetPositionVector2(hordeObjectInScene.transform.position, pos);

            //check if stop
            if ((useAnimationCurve && Time.time - startTime >= animationCurve.keys[animationCurve.keys.Length -1].time)     //last key time
                || (useAnimationCurve == false && delta >= 1))                                                              //delta reached 1
            {
                break;
            }

            yield return null;
        }
    }

    float GetPositionAxis(Vector2 position)
    {
        if (movementAxis == EAxis.X)
            return position.x;
        else
            return position.y;
    }

    Vector2 GetPositionVector2(Vector2 baseVector2, float axis)
    {
        if (movementAxis == EAxis.X)
            return new Vector2(axis, baseVector2.y);
        else
            return new Vector2(baseVector2.x, axis);
    }
}
