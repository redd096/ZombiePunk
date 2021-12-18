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
    public System.Action<GameObject> onSpawn { get; set; }
    public System.Action<Spawn> onFinishSpawn { get; set; }
    public System.Action<Spawn> onEveryObjectIsKilled { get; set; }

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
                onEveryObjectIsKilled?.Invoke(this);
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
