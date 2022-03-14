using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelActiver : MonoBehaviour
{
    
    public GameObject TilesOn;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TilesOn.SetActive(true);
        }
            
    }

        
}
