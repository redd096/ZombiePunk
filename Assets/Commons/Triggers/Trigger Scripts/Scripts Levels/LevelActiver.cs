using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelActiver : MonoBehaviour
{
    public GameObject Trigger;
    public GameObject TilesOn;

    void OnTriggerEnter2D(Collider2D other)
    {
        TilesOn.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Trigger.SetActive(false);
    }



}
