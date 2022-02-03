﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerLevelTrigger : MonoBehaviour
{
    public GameObject levelEnter, levelPlay;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   //private void OnTriggerEnter2D(Collider2D collision)
   //{
   //        levelPlay.SetActive(true);
   //        levelEnter.SetActive(false);
   //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        levelPlay.SetActive(true);
        levelEnter.SetActive(false);
        Destroy(gameObject);
    }
}
