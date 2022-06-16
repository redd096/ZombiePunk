using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelTriggers : MonoBehaviour
{
    public GameObject BarrelsOn;
    public GameObject BarrelsOff;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BarrelsOn.SetActive(true);
            BarrelsOff.SetActive(false);
        }
            
    }
}
