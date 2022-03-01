using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096;

public class AimFromTopFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] StateMachineRedd096 stateMachine = default;

    [Header("StateMachine - Aim From Top State")]
    [SerializeField] string AimFromTopStateName = "Aim From Top State";
    [SerializeField] string positionBlackboardName = "Last Target Position";

    [Header("Object to move")]
    [SerializeField] GameObject prefabObjectToMove = default;

    GameObject objectToMove;
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
        if (stateName == AimFromTopStateName)
        {
            moveObjectCoroutine = StartCoroutine(MoveObjectCoroutine());
        }
        //else stop coroutine
        else if(moveObjectCoroutine != null)
        {
            StopCoroutine(moveObjectCoroutine);
            moveObjectCoroutine = null;

            //deactive object
            if(objectToMove) objectToMove.SetActive(false);
        }
    }

    IEnumerator MoveObjectCoroutine()
    {
        //active object (update position before)
        if (objectToMove)
        {
            positionFromBlackboard = stateMachine.GetBlackboardElement<Vector2>(positionBlackboardName);
            objectToMove.transform.position = positionFromBlackboard;
            yield return null;
            objectToMove.SetActive(true);
        }

        while (true)
        {
            //get position and move object
            if (objectToMove)
            {
                positionFromBlackboard = stateMachine.GetBlackboardElement<Vector2>(positionBlackboardName);
                objectToMove.transform.position = positionFromBlackboard;
            }

            yield return null;
        }
    }
}
