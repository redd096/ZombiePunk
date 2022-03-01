using System.Collections;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Feedbacks/See Target Feedback")]
public class SeeTargetFeedback : MonoBehaviour
{
    [Header("Necessary Components - default in parent")]
    [SerializeField] StateMachineRedd096 stateMachine = default;

    [Header("StateMachine - Target Name in blackboard")]
    [SerializeField] string targetName = "Target";

    [Header("Object to activate (already in scene/prefab)")]
    [SerializeField] GameObject objectToActivate = default;
    [SerializeField] float durationBeforeDeactivate = 1.5f;

    Coroutine deactiveObjectCoroutine;

    void OnEnable()
    {
        //get references
        if (stateMachine == null)
        {
            Redd096Main main = GetComponentInParent<Redd096Main>();
            if (main) stateMachine = main.GetComponentInChildren<StateMachineRedd096>();
        }

        //add events
        if (stateMachine)
        {
            stateMachine.onSetBlackboardValue += OnSetBlackboardValue;
        }

        //deactive object by default
        objectToActivate.SetActive(false);
    }

    void OnDisable()
    {
        //remove events
        if (stateMachine)
        {
            stateMachine.onSetBlackboardValue -= OnSetBlackboardValue;
        }
    }

    void OnSetBlackboardValue(string keyName)
    {
        //if set value
        if (keyName == targetName)
        {
            if(objectToActivate)
            {
                //active object
                objectToActivate.SetActive(true);

                //start coroutine to deactive
                if (deactiveObjectCoroutine != null)
                    StopCoroutine(deactiveObjectCoroutine);

                deactiveObjectCoroutine = StartCoroutine(DeactiveObjectCoroutine());
            }
        }
    }

    IEnumerator DeactiveObjectCoroutine()
    {
        //wait
        yield return new WaitForSeconds(durationBeforeDeactivate);

        //deactive
        objectToActivate.SetActive(false);
    }
}
