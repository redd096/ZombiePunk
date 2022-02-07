using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastRoomTrigger : MonoBehaviour
{
    

    public GameObject Spawns;

    public GameObject Deactive;

    public GameObject Block;

    public GameObject Exit;

    void OnTriggerEnter2D(Collider2D other)
    {
        Spawns.SetActive(true);

        Destroy(Deactive);

    }

    void OnTriggerExit2D(Collider2D other)
    {
        Block.SetActive(true);

        Exit.SetActive(true);
    }

}
