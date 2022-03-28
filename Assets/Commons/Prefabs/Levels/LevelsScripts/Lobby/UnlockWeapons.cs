﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockWeapons : MonoBehaviour
{
    public float necessaryLevel;
    private float levelReach;

    // Start is called before the first frame update
    void Start()
    {
        levelReach = PlayerPrefs.GetFloat("LevelReach");

        if (necessaryLevel <= levelReach)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
