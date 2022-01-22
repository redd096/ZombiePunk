using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class ToDestroyS : MonoBehaviour
{
    private HealthComponent health;
    private DoorController door;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<HealthComponent>();
        print("Ok");
        door = GameObject.Find("Door").GetComponent<DoorController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.CurrentHealth <= 0)
        {
            print("BruttaMerda");
            door.objectsNumber -= 1;
        }
    }
}
