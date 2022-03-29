using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelActiverandDeactiver : MonoBehaviour
{
    
    public GameObject TilesOn;
    public GameObject TilesOff;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TilesOn.SetActive(true);
            TilesOff.SetActive(false);
        }
            
    }
}
