﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int objectsNumber;
    public GameObject turnOn, turnOff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (objectsNumber <= 0)
        {
            TurnOff();
        }
    }

    void TurnOff()
    {
        turnOn.SetActive(true);
        turnOff.SetActive(false);
    }
}