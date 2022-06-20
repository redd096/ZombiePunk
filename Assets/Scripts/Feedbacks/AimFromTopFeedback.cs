using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096;
using redd096.Attributes;

public class AimFromTopFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] StateMachineRedd096 stateMachine = default;

    [Header("StateMachine - Show during these states")]
    [SerializeField] System.Collections.Generic.List<string> statesWhenShow = new System.Collections.Generic.List<string>() { "Aim From Top State", "Move To Target State", "Attack State" };
    [SerializeField] string positionBlackboardName = "Last Target Position";

    [Header("Object to move")]
    [SerializeField] GameObject prefabObjectToMove = default;

    [Header("Object to resize")]
    [SerializeField] GameObject prefabObjectToResize = default;
    [SerializeField] float durationResize = 1;
    [SerializeField] float fromSize = 0;
    [SerializeField] bool useObjectToMoveSize = true;
    [HideIf("useObjectToMoveSize")] [SerializeField] float toSize = 1;

    GameObject objectToMove;
    GameObject objectToResize;
    Vector2 positionFromBlackboard;
    Coroutine moveObjectCoroutine;

    void OnEnable()
    {
        //get references
        if (stateMachine == null)
        {
            Redd096Main main = GetComponentInParent<Redd096Main>();
            if (main) stateMachine = main.GetComponentInChildren<StateMachineRedd096>();
        }

        //instantiate object to move
        if(objectToMove == null && prefabObjectToMove)
        {
            objectToMove = Instantiate(prefabObjectToMove);
            objectToMove.SetActive(false);
        }
        //and object to resize
        if (objectToResize == null && prefabObjectToResize)
        {
            objectToResize = Instantiate(prefabObjectToResize);
            objectToResize.SetActive(false);
        }

        //add events
        if (stateMachine)
        {
            stateMachine.onSetState += OnSetState;
        }
    }

    void OnDisable()
    {
        //remove events
        if (stateMachine)
        {
            stateMachine.onSetState -= OnSetState;
        }
    }

    void OnSetState(string stateName)
    {
        //if enter in state, start coroutine
        if (statesWhenShow.Contains(stateName))
        {
            if (moveObjectCoroutine == null)
                moveObjectCoroutine = StartCoroutine(MoveObjectCoroutine());
        }
        //else stop coroutine
        else if(moveObjectCoroutine != null)
        {
            StopCoroutine(moveObjectCoroutine);
            moveObjectCoroutine = null;

            //deactive objects
            if(objectToMove) objectToMove.SetActive(false);
            if (objectToResize) objectToResize.SetActive(false);
        }
    }

    IEnumerator MoveObjectCoroutine()
    {
        //update position and size before activate objects
        positionFromBlackboard = stateMachine.GetBlackboardElement<Vector2>(positionBlackboardName);
        if (objectToMove)
        {
            objectToMove.transform.position = positionFromBlackboard;
        }
        if (objectToResize)
        {
            objectToResize.transform.position = positionFromBlackboard;
            objectToResize.transform.localScale = new Vector2(fromSize, fromSize);
        }

        //activate objects
        yield return null;
        if (objectToMove) objectToMove.SetActive(true);
        if (objectToResize) objectToResize.SetActive(true);

        float deltaResize = 0;
        while (true)
        {
            //get position and move objects
            positionFromBlackboard = stateMachine.GetBlackboardElement<Vector2>(positionBlackboardName);
            if (objectToMove)
            {
                objectToMove.transform.position = positionFromBlackboard;
            }
            if (objectToResize)
            {
                objectToResize.transform.position = positionFromBlackboard;

                //and resize too
                if (deltaResize < 1)
                {
                    deltaResize += Time.deltaTime / durationResize;

                    objectToResize.transform.localScale = Vector2.Lerp(
                        new Vector2(fromSize, fromSize),
                        useObjectToMoveSize && objectToMove ? (Vector2)objectToMove.transform.localScale : new Vector2(toSize, toSize), 
                        deltaResize);
                }
            }

            yield return null;
        }
    }
}
