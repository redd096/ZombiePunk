using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//using NaughtyAttributes;
using redd096;

public class Spawn : MonoBehaviour
{
    [SerializeField] float timeBeforeFirstSpawn = 1;
    [SerializeField] float timeBetweenSpawns = 5;
    [SerializeField] GameObject[] prefabsToSpawn = default;

    [Header("DEBUG")]
    /*[ReadOnly] [SerializeField]*/ List<SpawnableObject> spawnedAlives = new List<SpawnableObject>();
    /*[ShowNonSerializedField]*/ int index = 0;

    //events
    public System.Action<GameObject> onSpawn { get; set; }                  //called when spawn an object
    public System.Action<Spawn> onFinishSpawn { get; set; }                 //called when finish to spawn every object
    public System.Action<Spawn> onEveryObjectIsDeactivated { get; set; }    //called when finish to spawn and every object is been deactivated

    //used in spawn object
    SpawnFeedback spawnFeedback;
    Coroutine spawnCoroutine;

    public void StartSpawn()
    {
        //get references
        if (spawnFeedback == null) spawnFeedback = GetComponent<SpawnFeedback>();

        //be sure to stop coroutine
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        //start coroutine
        if (gameObject.activeInHierarchy)
            spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        for (index = 0; index < prefabsToSpawn.Length; index++)
        {
            //wait
            yield return new WaitForSeconds(index <= 0 ? timeBeforeFirstSpawn : timeBetweenSpawns);

            //then spawn
            SpawnObject(prefabsToSpawn[index]);
        }

        //call event
        onFinishSpawn?.Invoke(this);

        spawnCoroutine = null;

        //call the function to check: if there aren't objects active, call the event where every object is deactivated
        OnDeactiveObject(null);
    }

    void OnDeactiveObject(SpawnableObject spawnableObject)
    {
        //remove from the list and unregister from event
        if(spawnedAlives.Contains(spawnableObject))
        {
            spawnedAlives.Remove(spawnableObject);
            if(spawnableObject) spawnableObject.onDeactiveObject -= OnDeactiveObject;
        }
        
        //if has finished to spawn, check if every spawned object was deactivated
        if(spawnCoroutine == null)
        {
            if(spawnedAlives.Count <= 0)
            {
                //call event
                onEveryObjectIsDeactivated?.Invoke(this);
            }
        }
    }

    #region private API

    void SpawnObject(GameObject objectToSpawn)
    {
        GameObject instantiatedObject;
        if (objectToSpawn)
        {
            //spawn and set parent
            instantiatedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);
            instantiatedObject.transform.SetParent(transform);

            //if there is spawn feedback, deactive because will be activated from SpawnFeedback
            if (spawnFeedback)
            {
                instantiatedObject.SetActive(false);
            }

            //if has health, register to events and add to list
            SpawnableObject spawnableObject = instantiatedObject.AddComponent<SpawnableObject>();
            if (spawnableObject)
            {
                spawnedAlives.Add(spawnableObject);
                spawnableObject.onDeactiveObject += OnDeactiveObject;
            }

            //call event
            onSpawn?.Invoke(instantiatedObject);
        }
    }

    #endregion
}
