using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDeactiver : MonoBehaviour
{
    public GameObject Trigger;
    public GameObject TilesOff;

    void OnTriggerEnter2D(Collider2D other)
    {
        TilesOff.SetActive(false);
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        Trigger.SetActive(false);
    }
}
