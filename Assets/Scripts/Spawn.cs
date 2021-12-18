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
    public System.Action<Spawn> onEveryObjectIsDead { get; set; }     //called when finish to spawn and every object with health component is dead (if there aren't, is called when finish to spawn)

    Coroutine spawnCoroutine;

    void OnEnable()
    {
        //start coroutine
        spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    void OnDisable()
    {
        //be sure to stop coroutine
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }

    IEnumerator SpawnCoroutine()
    {
        for(index = 0; index < prefabsToSpawn.Length; index++)
        {
            //wait
            yield return new WaitForSeconds(index <= 0 ? timeBeforeFirstSpawn : timeBetweenSpawns);

            //then spawn
            SpawnObject();
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
            whoDied.onDie -= OnKilledObject;
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

    void SpawnObject()
    {
        if (prefabsToSpawn[index])
        {
            //spawn
            GameObject go = Instantiate(prefabsToSpawn[index], transform.position, transform.rotation);
            go.transform.SetParent(transform);

            //if has health, register to events and add to list
            HealthComponent health = go.GetComponent<HealthComponent>();
            if (health)
            {
                health.onDie += OnKilledObject;
                spawnedAlives.Add(health);
            }

            //call event
            onSpawn?.Invoke(go);
        }
    }

    #endregion
}
