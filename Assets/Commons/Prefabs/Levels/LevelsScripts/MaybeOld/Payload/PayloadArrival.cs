﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadArrival : MonoBehaviour
{
    public GameObject door;

    private GameObject payload;
    private PayloadMovement script;
    
    // Start is called before the first frame update
    void Start()
    {
        payload = GameObject.Find("Payload");
        script = GameObject.Find("Payload").GetComponent<PayloadMovement>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.x <= payload.transform.position.x)
        {
            Destroy(script);
            Destroy(door);
        }
    }
}