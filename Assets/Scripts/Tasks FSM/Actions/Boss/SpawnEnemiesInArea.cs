using System.Collections;
using UnityEngine;
using redd096;
using redd096.Attributes;

[System.Serializable]
public struct SpawnEnemiesStruct
{
    public GameObject EnemyPrefab;
    [Range(0, 100)] public int PercentageSpawn;
}

[AddComponentMenu("redd096/Tasks FSM/Action/Boss/Spawn Enemies In Area")]
public class SpawnEnemiesInArea : ActionTask
{
    [Header("Enemies to Spawn")]
    [SerializeField] int minEnemiesToSpawn = 3;
    [SerializeField] int maxEnemiesToSpawn = 10;
    [SerializeField] float minTimeBetweenSpawns = 0.5f;
    [SerializeField] float maxTimeBetweenSpawns = 1.5f;
    [SerializeField] float timeToWaitAfterFinishedToSpawn = 2;

    [Header("Spawn Area")]
    [SerializeField] Vector2 centerSpawnArea = Vector2.zero;
    [SerializeField] Vector2 sizeSpawnArea = Vector2.one * 3;

    [Header("List of prefabs to spawn")]
    [ReadOnly] [SerializeField] int totalPercentage = 0;
    [SerializeField] SpawnEnemiesStruct[] enemiesToSpawn = default;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    Coroutine spawnEnemiesCoroutine;

    void OnValidate()
    {
        //editor - show total percentage
        totalPercentage = 0;
        if (enemiesToSpawn != null && enemiesToSpawn.Length > 0)
        {
            foreach (SpawnEnemiesStruct enemyStruct in enemiesToSpawn)
                totalPercentage += enemyStruct.PercentageSpawn;
        }
    }

    void OnDrawGizmos()
    {
        if (drawDebug)
        {
            //draw spawn area
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transformTask.position + centerSpawnArea, sizeSpawnArea);
            Gizmos.color = Color.white;
        }
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //start spawn coroutine
        if (spawnEnemiesCoroutine != null)
            StopCoroutine(spawnEnemiesCoroutine);

        spawnEnemiesCoroutine = StartCoroutine(SpawnEnemiesCoroutine());
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //be sure to stop coroutine if exit from this task
        if (spawnEnemiesCoroutine != null)
            StopCoroutine(spawnEnemiesCoroutine);
    }

    #region private API

    IEnumerator SpawnEnemiesCoroutine()
    {
        //calculate number of enemies to spawn
        int randomEnemies = Random.Range(minEnemiesToSpawn, maxEnemiesToSpawn);

        //instantiate every enemy
        for (int i = 0; i < randomEnemies; i++)
        {
            //wait between spawns
            yield return new WaitForSeconds(Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns));

            InstantiateEnemy();
        }

        //when finish to spawn every enemy, wait few seconds then call complete task
        yield return new WaitForSeconds(timeToWaitAfterFinishedToSpawn);
        CompleteTask();
    }

    void InstantiateEnemy()
    {
        //get random value from 0 to 100
        int random = Mathf.RoundToInt(Random.value * 100);

        //calculate random position
        float x = Random.Range(centerSpawnArea.x - sizeSpawnArea.x * 0.5f, centerSpawnArea.x + sizeSpawnArea.x * 0.5f);
        float y = Random.Range(centerSpawnArea.y - sizeSpawnArea.y * 0.5f, centerSpawnArea.y + sizeSpawnArea.y * 0.5f);

        //cycle every enemy
        int percentage = 0;
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            //if reach percentage, instantiate
            percentage += enemiesToSpawn[i].PercentageSpawn;
            if (percentage >= random)
            {
                if (enemiesToSpawn[i].EnemyPrefab)
                {
                    Instantiate(enemiesToSpawn[i].EnemyPrefab, (Vector2)transformTask.position + new Vector2(x, y), Quaternion.identity);
                }

                break;
            }
        }
    }

    #endregion
}
