using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class ToDestroyS : MonoBehaviour
{
    public DoorController door;

    // Start is called before the first frame update
    void Start()
    {
        //door = GameObject.Find("Door").GetComponent<DoorController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        print("BruttaMerda");
        door.objectsNumber -= 1;
    }
}
