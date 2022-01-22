using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWave : MonoBehaviour
{
    public GameObject Player;

    public GameObject Spawns;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Spawns.SetActive(true);

    }


}
