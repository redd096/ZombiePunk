using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096.Attributes;

[DefaultExecutionOrder(-1)]
public class SpawnManager : MonoBehaviour
{
    //structs
    [System.Serializable] struct SpawnStruct { public Spawn[] Spawns; }

    [SerializeField] bool restartWhenFinish = false;
    [DisableIf("restartWhenFinish")] [Tooltip("Remember it can't be used from exits, if restart when finish is enabled")] [SerializeField] bool exitMustCheckThisSpawnManager = true;
    [SerializeField] SpawnStruct[] spawnsList = default;

    [Header("DEBUG")]
    /*[ReadOnly] [SerializeField]*/ List<Spawn> spawnsActive = new List<Spawn>();
    /*[ShowNonSerializedField]*/ int index = 0;

    public bool RestartWhenFinish => restartWhenFinish;
    public bool ExitMustCheckThisSpawnManager => exitMustCheckThisSpawnManager;

    //events
    public System.Action<Spawn[]> onActiveList { get; set; }                //called when active a list of spawns
    public System.Action<Spawn[]> onFinishList { get; set; }                //called when finish a list of spawns
    public System.Action<GameObject> onEverySpawn { get; set; }             //called when one Spawn spawn an object
    public System.Action<SpawnManager> onFinishEveryList { get; set; }      //called when finish all the lists
    public System.Action<SpawnManager> onRestart { get; set; }              //called when finish all the lists and restart from the first
    public System.Action<SpawnManager> onFinishToSpawn { get; set; }        //called when finish all the lists and stops

    void OnEnable()
    {
        //add to level manager if not already in the list (if spawned at runtime)
        if (GameManager.instance && GameManager.instance.levelManager)
        {
            GameManager.instance.levelManager.AddSpawnedSpawnManager(this);
        }
    }

    void Start()
    {
        //register to every spawn event
        foreach (SpawnStruct spawnStruct in spawnsList)
        {
            foreach (Spawn spawn in spawnStruct.Spawns)
            {
                if (spawn)
                    spawn.onSpawn += OnEverySpawn;
            }
        }

        //active first list
        ActiveList();
    }

    void OnKilledEverySpawnedObject(Spawn spawn)
    {
        //remove from the list and unregister from events
        if(spawnsActive.Contains(spawn))
        {
            spawnsActive.Remove(spawn);
            spawn.onEveryObjectIsDeactivated -= OnKilledEverySpawnedObject;
        }

        //check if finish to spawn this list
        CheckFinishList();
    }

    void CheckFinishList()
    {
        //if finished
        if (spawnsActive.Count <= 0)
        {
            //call event on finish list
            onFinishList?.Invoke(spawnsList[index].Spawns);

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

    void ActiveList()
    {
        //active every spawn and register to their events
        foreach (Spawn spawn in spawnsList[index].Spawns)
        {
            if (spawn == null)
                continue;

            spawn.onEveryObjectIsDeactivated += OnKilledEverySpawnedObject;
            spawn.StartSpawn();

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

    void OnEverySpawn(GameObject spawnedObject)
    {
        //call event for each spawned object
        onEverySpawn?.Invoke(spawnedObject);
    }

    #endregion
}
