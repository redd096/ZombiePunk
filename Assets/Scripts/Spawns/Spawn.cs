using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using redd096;

public class Spawn : MonoBehaviour
{
    [SerializeField] float timeBeforeFirstSpawn = 1;
    [SerializeField] float timeBetweenSpawns = 5;
    [SerializeField] GameObject[] prefabsToSpawn = default;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] List<HealthComponent> spawnedAlives = new List<HealthComponent>();
    [ShowNonSerializedField] int index = 0;

    //events
    public System.Action<GameObject> onSpawn { get; set; }              //called when spawn an object
    public System.Action<Spawn> onFinishSpawn { get; set; }             //called when finish to spawn every object
    public System.Action<Spawn> onEveryObjectIsDead { get; set; }       //called when finish to spawn and every object with health component is dead (if there aren't, is called when finish to spawn)

    //used in spawn object
    Coroutine instantiateCoroutine;
    Coroutine spawnCoroutine;
    List<GameObject> instantiatedObjectsToSpawn = new List<GameObject>();

    void Start()
    {
        //at start instantiate every prefab and deactive
        instantiateCoroutine = StartCoroutine(InstantiateEveryPrefab());
    }

    public void StartSpawn()
    {
        //be sure to stop coroutine
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        //start coroutine
        spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        //wait to be sure instantiate coroutine is started
        yield return null;

        //wait to finish instantiate
        while (instantiateCoroutine != null)
            yield return null;

        for (index = 0; index < instantiatedObjectsToSpawn.Count; index++)
        {
            //wait
            yield return new WaitForSeconds(index <= 0 ? timeBeforeFirstSpawn : timeBetweenSpawns);

            //then spawn
            SpawnObject(instantiatedObjectsToSpawn[index]);
        }

        //call event
        onFinishSpawn?.Invoke(this);

        spawnCoroutine = null;

        //call the function to check: if there aren't objects with health component, call the event where every object is dead
        OnKilledObject(null);
    }

    void OnKilledObject(HealthComponent whoDied)
    {
        //remove from the list and unregister from event
        if(spawnedAlives.Contains(whoDied))
        {
            spawnedAlives.Remove(whoDied);
            if(whoDied) whoDied.onDie -= OnKilledObject;
        }
        
        //if has finished to spawn, check if every spawned object was killed
        if(spawnCoroutine == null)
        {
            if(spawnedAlives.Count <= 0)
            {
                //call event
                onEveryObjectIsDead?.Invoke(this);
            }
        }
    }

    #region private API

    IEnumerator InstantiateEveryPrefab()
    {
        GameObject instantiatedObject;
        foreach (GameObject go in prefabsToSpawn)
        {
            if (go == null)
                continue;

            //spawn and deactive
            instantiatedObject = Instantiate(go, transform.position, transform.rotation);
            instantiatedObject.transform.SetParent(transform);
            instantiatedObject.SetActive(false);

            //add to list
            instantiatedObjectsToSpawn.Add(instantiatedObject);

            yield return null;
        }

        instantiateCoroutine = null;
    }

    void SpawnObject(GameObject objectToSpawn)
    {
        if (objectToSpawn)
        {
            //spawn
            objectToSpawn.SetActive(true);

            //if has health, register to events and add to list
            HealthComponent healthInstantiatedObject = objectToSpawn.GetComponent<HealthComponent>();
            if (healthInstantiatedObject)
            {
                healthInstantiatedObject.onDie += OnKilledObject;
                spawnedAlives.Add(healthInstantiatedObject);
            }

            //call event
            onSpawn?.Invoke(objectToSpawn);
        }
    }

    #endregion
}
