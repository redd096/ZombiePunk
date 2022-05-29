using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;

public class AimLineFeedback : MonoBehaviour
{
    [Header("Necessary components - default get in parent")]
    [SerializeField] AimComponent aimComponent = default;
    [SerializeField] AimAtTarget[] aimAtTargetTasks = default;

    [Header("Line Feedback")]
    [SerializeField] LineRenderer linePrefab = default;
    [SerializeField] float distanceLine = 4;

    LineRenderer line;
    Coroutine showLineCoroutine;

    void OnEnable()
    {
        //get references
        if (aimComponent == null) GetComponentInParent<AimComponent>();
        if (aimAtTargetTasks == null || aimAtTargetTasks.Length <= 0) GetComponentInParent<Character>()?.GetComponentsInChildren<AimAtTarget>();

        //instantiate line and set child
        if (linePrefab)
        {
            line = Instantiate(linePrefab, transform);

            //be sure to not see line
            HideLine();
        }

        //add events
        if (aimAtTargetTasks != null)
        {
            foreach (AimAtTarget aimAtTarget in aimAtTargetTasks)
            {
                if (aimAtTarget)
                {
                    aimAtTarget.onStartAimAtTarget += OnStartAimAtTarget;
                    aimAtTarget.onEndAimAtTarget += OnEndAimAtTarget;
                }
            }
        }
    }

    void OnDisable()
    {
        //remove events
        if (aimAtTargetTasks != null)
        {
            foreach (AimAtTarget aimAtTarget in aimAtTargetTasks)
            {
                if (aimAtTarget)
                {
                    aimAtTarget.onStartAimAtTarget -= OnStartAimAtTarget;
                    aimAtTarget.onEndAimAtTarget -= OnEndAimAtTarget;
                }
            }
        }
    }

    #region private API

    void OnStartAimAtTarget()
    {
        //start coroutine
        if (showLineCoroutine != null)
            StopCoroutine(showLineCoroutine);

        showLineCoroutine = StartCoroutine(ShowLineCoroutine());
    }

    void OnEndAimAtTarget()
    {
        //stop coroutine
        if (showLineCoroutine != null)
            StopCoroutine(showLineCoroutine);

        //hide line
        HideLine();
    }

    IEnumerator ShowLineCoroutine()
    {
        //set 2 points in line
        if (line)
            line.positionCount = 2;

        //continue update line positions
        while (true)
        {
            if (line)
            {
                //from transform position to position + aim direction (if no aim component, set only transform position to not show line)
                line.SetPosition(0, (Vector2)transform.position);
                line.SetPosition(1, (Vector2)transform.position + (aimComponent ? aimComponent.AimDirectionInput * distanceLine : Vector2.zero));

                yield return null;
            }
        }
    }

    void HideLine()
    {
        //set positions at zero to not see it
        line.positionCount = 2;
        line.SetPosition(0, Vector2.zero);
        line.SetPosition(1, Vector2.zero);
    }

    #endregion
}
