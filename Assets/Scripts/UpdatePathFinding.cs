using System.Collections;
using UnityEngine;
using redd096;
using NaughtyAttributes;

public class UpdatePathFinding : MonoBehaviour
{
    enum EUpdateModes { Update, FixedUpdate, Coroutine, None }

    [Header("Necessary Components - default get from this gameObject")]
    [Tooltip("To update the grid")] [SerializeField] ObstacleAStar2D component;
    [Tooltip("To update when is moving")] [SerializeField] MovementComponent movementComponent;
    [Tooltip("To update when is dead")] [SerializeField] HealthComponent healthComponent;

    [Header("Updates")]
    [Tooltip("Update or FixedUpdate?")] [SerializeField] EUpdateModes updateMode = EUpdateModes.Coroutine;
    [Tooltip("Delay between updates using Coroutine method")] [EnableIf("updateMode", EUpdateModes.Coroutine)] [SerializeField] float timeCoroutine = 0.1f;
    [SerializeField] bool updateOnStart = true;
    [SerializeField] bool updateOnMoves = true;
    [SerializeField] bool updateOnDeath = true;

    //update mode
    Coroutine updateCoroutine;

    void OnEnable()
    {
        //get references
        if (component == null) component = GetComponent<ObstacleAStar2D>();
        if (movementComponent == null) movementComponent = GetComponent<MovementComponent>();
        if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();

        if (component == null) Debug.LogWarning("Missing ObstacleAStar2D on " + name);
        if (updateOnMoves && movementComponent == null) Debug.LogWarning("Missing MovementComponent on " + name);
        if (updateOnDeath && healthComponent == null) Debug.LogWarning("Missing HealthComponent on " + name);

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

    void Start()
    {
        //update on start
        if (updateOnStart)
            UpdatePath(true);
    }

    void Update()
    {
        //do only if update mode is Update
        if (updateMode == EUpdateModes.Update)
        {
            //update on moves
            if(updateOnMoves && movementComponent && movementComponent.CurrentSpeed > 0)
                UpdatePath();
        }
    }

    void FixedUpdate()
    {
        //do only if update mode is FixedUpdate
        if (updateMode == EUpdateModes.FixedUpdate)
        {
            //update on moves
            if (updateOnMoves && movementComponent && movementComponent.CurrentSpeed > 0)
                UpdatePath();
        }
    }

    IEnumerator UpdateCoroutine()
    {
        //do only if update mode is Coroutine
        while (updateMode == EUpdateModes.Coroutine)
        {
            //update on moves
            if (updateOnMoves && movementComponent && movementComponent.CurrentSpeed > 0)
                UpdatePath();

            yield return new WaitForSeconds(timeCoroutine);
        }
    }

    void OnDie(HealthComponent whoDied)
    {
        //update on death (remove colliders to be sure is not calculated again)
        if (updateOnDeath)
        {
            if (component)
            {
                component.RemoveColliders();
            }

            UpdatePath();
        }
    }

    void UpdatePath(bool updateImmediatly = false)
    {
        //update path
        if (GameManager.instance && GameManager.instance.pathFindingAStar)
        {
            if(component)
                GameManager.instance.pathFindingAStar.UpdateGrid(component, updateImmediatly);
        }
    }
}
