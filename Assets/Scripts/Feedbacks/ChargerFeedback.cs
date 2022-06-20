using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096;

public class ChargerFeedback : MonoBehaviour
{
    [Header("Necessary components - default get in parent")]
    [SerializeField] AimComponent aimComponent = default;
    [SerializeField] StateMachineRedd096 stateMachine = default;

    [Header("Objects to activate when aim (already in scene)")]
    [SerializeField] List<string> statesWhenActivate = new List<string>() { "DelayState" };
    [SerializeField] GameObject[] objectsToActivate = default;

    [Header("Change color after X seconds")]
    [SerializeField] float secondsBeforeChangeColor = 1;
    [SerializeField] Color colorToUse = Color.red;

    Coroutine aimCoroutine;
    Dictionary<SpriteRenderer, Color> defaultColors = new Dictionary<SpriteRenderer, Color>();

    void Awake()
    {
        //save default colors and deactivate objects by default
        foreach (GameObject go in objectsToActivate)
        {
            if (go)
            {
                foreach (SpriteRenderer sprite in go.GetComponentsInChildren<SpriteRenderer>())
                    if (defaultColors.ContainsKey(sprite) == false)
                        defaultColors.Add(sprite, sprite.color);

                go.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        //get references
        if (aimComponent == null) GetComponentInParent<AimComponent>();
        if (stateMachine == null)
        {
            Redd096Main main = GetComponentInParent<Redd096Main>();
            if (main) stateMachine = main.GetComponentInChildren<StateMachineRedd096>();
        }

        //add events
        if (stateMachine)
        {
            stateMachine.onSetState += OnSetState;
            OnSetState(stateMachine.CurrentState.StateName);    //set default
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

    private void OnSetState(string stateName)
    {
        bool activate = statesWhenActivate.Contains(stateName);

        //if activate, start coroutine
        if (activate && aimCoroutine == null)
        {
            aimCoroutine = StartCoroutine(AimCoroutine());
        }
        //if deactive
        else if (activate == false)
        {
            //stop coroutine
            if (aimCoroutine != null)
            {
                StopCoroutine(aimCoroutine);
                aimCoroutine = null;

                //deactivate objects
                foreach (GameObject go in objectsToActivate)
                    if (go)
                        go.SetActive(false);
            }
        }
    }

    IEnumerator AimCoroutine()
    {
        //set default colors
        foreach (SpriteRenderer sprite in defaultColors.Keys)
            if (sprite)
                sprite.color = defaultColors[sprite];

        //set rotation and activate object
        Quaternion rotation = GetRotation();
        foreach (GameObject go in objectsToActivate)
        {
            if (go)
            {
                go.transform.rotation = rotation;
                go.SetActive(true);
            }
        }

        float timerChangeColor = Time.time + secondsBeforeChangeColor;
        while (true)
        {
            //continue rotate until stop coroutine
            rotation = GetRotation();
            foreach (GameObject go in objectsToActivate)
            {
                if (go)
                {
                    go.transform.rotation = rotation;
                }
            }

            //after few seconds, change color
            if (Time.time > timerChangeColor && timerChangeColor > 0)
            {
                foreach (SpriteRenderer sprite in defaultColors.Keys)
                    if (sprite)
                        sprite.color = colorToUse;

                //set -1 to not change again color
                timerChangeColor = -1;  
            }

            yield return null;
        }
    }

    Quaternion GetRotation()
    {
        //rotation
        Vector2 direction = aimComponent ? aimComponent.AimDirectionInput : Vector2.right; //((Vector2)transform.position - hitPoint).normalized;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * direction);   //Forward keep to Z axis. Up use X instead of Y (AngleAxis 90) and rotate to direction

        //when rotate to opposite direction (from default), rotate 180 updown
        if (direction.x < 0)
        {
            rotation *= Quaternion.AngleAxis(180, Vector3.right);
        }

        return rotation;
    }
}
