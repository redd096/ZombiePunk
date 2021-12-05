using System.Collections;
using UnityEngine;
using redd096;
using NaughtyAttributes;

public class UpdatePathFinding : MonoBehaviour
{
    enum EUpdateModes { Update, FixedUpdate, Coroutine, None }

    [Header("Necessary Components - default get from this gameObject")]
    [Tooltip("To update when is moving")] [SerializeField] MovementComponent component;
    [Tooltip("To update when is dead")] [SerializeField] HealthComponent healthComponent;

    [Header("Updates")]
    [Tooltip("Update or FixedUpdate?")] [SerializeField] EUpdateModes updateMode = EUpdateModes.Coroutine;
    [Tooltip("Delay between updates using Coroutine method")] [EnableIf("updateMode", EUpdateModes.Coroutine)] [SerializeField] float timeCoroutine = 0.1f;
    [SerializeField] bool updateOnMoves = true;
    [SerializeField] bool updateOnDeath = true;

    //update mode
    Coroutine updateCoroutine;

    void OnEnable()
    {
        //get references
        if (component == null) component = GetComponent<MovementComponent>();
        if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();

        if (component == null) Debug.LogWarning("Missing MovementComponent on " + name);
        if (healthComponent == null) Debug.LogWarning("Missing HealthComponent on " + name);

        //add events
        if (healthComponent)
        {
            healthComponent.onDie += OnDie;
        }

        //start coroutine
        if (updateMode == EUpdateModes.Coroutine)
            updateCoroutine = StartCoroutine(UpdateCoroutine());
    }

    void OnDisable()
    {
        //remove events
        if (healthComponent)
        {
            healthComponent.onDie -= OnDie;
        }

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
        {
            //update on moves
            if(updateOnMoves && component && component.CurrentSpeed > 0)
                UpdatePath();
        }
    }

    void FixedUpdate()
    {
        //do only if update mode is FixedUpdate
        if (updateMode == EUpdateModes.FixedUpdate)
        {
            //update on moves
            if (updateOnMoves && component && component.CurrentSpeed > 0)
                UpdatePath();
        }
    }

    IEnumerator UpdateCoroutine()
    {
        //do only if update mode is Coroutine
        while (updateMode == EUpdateModes.Coroutine)
        {
            //update on moves
            if (updateOnMoves && component && component.CurrentSpeed > 0)
                UpdatePath();

            yield return new WaitForSeconds(timeCoroutine);
        }
    }

    void OnDie(Redd096Main whoDied)
    {
        //update on death (update with a delay, to be sure this gameObject is destroyed)
        if (updateOnDeath)
            UpdatePath(1);
    }

    void UpdatePath(float delay = 0)
    {
        //update path
        if (GameManager.instance && GameManager.instance.pathFindingAStar)
        {
            if(delay <= 0)
                GameManager.instance.pathFindingAStar.UpdateGrid();
            else
                GameManager.instance.pathFindingAStar.Invoke("UpdateGrid", 1);      //update with a delay
        }
    }
}
