using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWave : MonoBehaviour
{
    public GameObject spawns;
    public GameObject deactive;
    public GameObject block;




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spawns.SetActive(true);
            Destroy(deactive);
        }
            

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            block.SetActive(true);
        }

            
    }



}
