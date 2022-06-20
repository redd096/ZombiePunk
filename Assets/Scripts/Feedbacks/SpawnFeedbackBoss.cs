using System.Collections;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Feedbacks/Spawn Feedback Boss")]
public class SpawnFeedbackBoss : MonoBehaviour
{
    [Header("Spawn vfx before spawn object")]
    [SerializeField] InstantiatedGameObjectStruct vfxToInstantiateBeforeSpawn = default;
    [SerializeField] float spawnObjectAfter = 0.5f;

    SpawnEnemiesInArea spawn;

    void OnEnable()
    {
        //get references
        if (spawn == null)
        {
            Redd096Main main = GetComponentInParent<Redd096Main>();
            if (main) spawn = main.GetComponentInChildren<SpawnEnemiesInArea>();
        }

        //add events
        if (spawn)
        {
            spawn.onSpawn += OnSpawn;
        }
    }

    void OnDisable()
    {
        //remove events
        if (spawn)
        {
            spawn.onSpawn -= OnSpawn;
        }
    }

    void OnSpawn(GameObject objectToSpawn)
    {
        StartCoroutine(SpawnCoroutine(objectToSpawn));
    }

    IEnumerator SpawnCoroutine(GameObject objectToSpawn)
    {
        //instantiate vfx
        if (objectToSpawn)
            InstantiateGameObjectManager.instance.Play(vfxToInstantiateBeforeSpawn, objectToSpawn.transform.position, objectToSpawn.transform.rotation);

        //wait
        yield return new WaitForSeconds(spawnObjectAfter);

        //and instantiate
        if (objectToSpawn)
            objectToSpawn.SetActive(true);
    }
}
