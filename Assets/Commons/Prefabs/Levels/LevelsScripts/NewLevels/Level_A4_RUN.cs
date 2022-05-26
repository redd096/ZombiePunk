using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Level_A4_RUN : MonoBehaviour
{
    private Level_A3_Storage storage;

    // Start is called before the first frame update
    void Start()
    {
        storage = GameObject.Find("Storage").GetComponent<Level_A3_Storage>();

        //storage.killAll.SetActive(false);
        //storage.play.SetActive(false);
        //storage.exit.SetActive(true);
        //storage.gatesOfHellOff.SetActive(true);
        //storage.gatesOfHellOn.SetActive(false);
        //storage.hell.SetActive(true);
        storage.level1.SetActive(false);
        storage.level3.SetActive(true);

        FindObjectOfType<AstarPath>().Scan();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
