using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[DefaultExecutionOrder(-1)]
public class SpawnManager : MonoBehaviour
{
    //structs
    [System.Serializable] struct SpawnStruct { public Spawn[] Spawns; }

    [SerializeField] bool restartWhenFinish = false;
    [ReorderableList] [SerializeField] SpawnStruct[] spawnsList = default;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] List<Spawn> spawnsActive = new List<Spawn>();
    [ShowNonSerializedField] int index = 0;

    //events
    public System.Action<Spawn[]> onActiveList { get; set; }                //called when active a list of spawns
    public System.Action<Spawn[]> onFinishList { get; set; }                //called when finish a list of spawns
    public System.Action<SpawnManager> onFinishEveryList { get; set; }      //called when finish all the lists
    public System.Action<SpawnManager> onRestart { get; set; }              //called when finish all the lists and restart from the first
    public System.Action<SpawnManager> onFinishToSpawn { get; set; }        //called when finish all the lists and stops


    void Awake()
    {
        //deactive every spawn
        for(int i = 0; i < spawnsList.Length; i++)
        {
            foreach (Spawn spawn in spawnsList[i].Spawns)
            {
                if (spawn == null)
                    continue;

                spawn.gameObject.SetActive(false);
            }
        }

        //and active first list
        ActiveList();
    }

    void OnKilledEverySpawnedObject(Spawn spawn)
    {
        //remove from the list and unregister from events
        if(spawnsActive.Contains(spawn))
        {
            spawnsActive.Remove(spawn);
            spawn.onEveryObjectIsKilled -= OnKilledEverySpawnedObject;
        }

        //check if finish to spawn this list
        CheckFinishList();
    }

    void CheckFinishList()
    {
        //if finished
        if (spawnsActive.Count <= 0)
        {
            //deactive every spawn
            FinishList();

            //active next list of spawns
            if (index + 1 < spawnsList.Length)
            {
                index++;
                ActiveList();
            }
            //if there aren't others, finish to spawn or restart
            else
            {
                FinishEveryList();
            }
        }
    }

    #region private API

    void FinishList()
    {
        //call event on finish list
        onFinishList?.Invoke(spawnsList[index].Spawns);

        //deactive every spawn
        foreach(Spawn spawn in spawnsList[index].Spawns)
        {
            if (spawn == null)
                continue;

            spawn.gameObject.SetActive(false);
        }
    }

    void ActiveList()
    {
        //active every spawn and register to their events
        foreach (Spawn spawn in spawnsList[index].Spawns)
        {
            if (spawn == null)
                continue;

            spawn.gameObject.SetActive(true);
            spawn.onEveryObjectIsKilled += OnKilledEverySpawnedObject;

            //add to list, to know when every spawn of this list has finished
            spawnsActive.Add(spawn);
        }

        //call event on active list
        onActiveList?.Invoke(spawnsList[index].Spawns);
    }

    void FinishEveryList()
    {
        //call event on finish every list
        onFinishEveryList?.Invoke(this);

        //restart from the first
        if(restartWhenFinish)
        {
            onRestart?.Invoke(this);

            index = 0;
            ActiveList();
        }
        //or finish to spawn
        else
        {
            onFinishToSpawn?.Invoke(this);
        }
    }

    #endregion
}
