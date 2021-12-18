using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnTest : MonoBehaviour
{
    public GameObject zombie;
    public float startDealy, interval;

    private Transform spawn1, spawn2, spawn3, spawn4;

    // Start is called before the first frame update
    void Start()
    {
        spawn1 = GameObject.Find("spawn1").GetComponent<Transform>();
        spawn2 = GameObject.Find("spawn2").GetComponent<Transform>();
        spawn3 = GameObject.Find("spawn3").GetComponent<Transform>();
        spawn4 = GameObject.Find("spawn4").GetComponent<Transform>();

        InvokeRepeating("SpawnZombies", startDealy, interval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnZombies()
    {
        Instantiate(zombie, spawn1.transform);
        Instantiate(zombie, spawn2.transform);
        Instantiate(zombie, spawn3.transform);
        Instantiate(zombie, spawn4.transform);
    }
}
