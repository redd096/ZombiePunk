using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    public GameObject Player;

    public GameObject Spawns;

    public GameObject Trigger;

    void OnTriggerEnter2D(Collider2D other)
    {
        Spawns.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Trigger.SetActive(false);
    }

}
