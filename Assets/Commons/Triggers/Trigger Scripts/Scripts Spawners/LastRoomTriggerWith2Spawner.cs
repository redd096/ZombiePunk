using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastRoomTriggerWith2Spawner : MonoBehaviour
{
    public GameObject Spawn1, Spawn2;

    public GameObject Deactive;

    public GameObject Block;

    public GameObject Exit;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Spawn1.SetActive(true);
            Spawn2.SetActive(true);

            Destroy(Deactive);
        }
            

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Block.SetActive(true);

            Exit.SetActive(true);
        }
            
    }

}
