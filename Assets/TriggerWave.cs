using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWave : MonoBehaviour
{
    public GameObject player;
    public GameObject spawns;
    public GameObject deactive;
    public GameObject block;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        spawns.SetActive(true);
        Destroy(deactive);

    }

    void OnTriggerExit2D(Collider2D other)
    {
        block.SetActive(true);
    }



}
