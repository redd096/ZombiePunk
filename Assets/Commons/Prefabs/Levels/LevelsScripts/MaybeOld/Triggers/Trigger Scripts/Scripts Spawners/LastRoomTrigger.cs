using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastRoomTrigger : MonoBehaviour
{
    

    public GameObject Spawns;

    public GameObject Deactive;

    public GameObject Block;

    public GameObject Exit;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Spawns.SetActive(true);

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
