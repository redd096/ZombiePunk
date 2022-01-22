using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWave : MonoBehaviour
{
    public GameObject Player;

    public GameObject Spawns;

    public GameObject Deactive;

    public GameObject Block;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Spawns.SetActive(true);

        Deactive.SetActive(false);

    }

    void OnTriggerExit2D(Collider2D other)
    {
        Block.SetActive(true);
    }



}
