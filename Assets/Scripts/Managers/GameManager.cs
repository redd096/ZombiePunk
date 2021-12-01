﻿using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Singletons/Game Manager")]
[DefaultExecutionOrder(-100)]
public class GameManager : Singleton<GameManager>
{
    public UIManager uiManager { get; private set; }
    public PathFindingAStar pathFindingAStar { get; private set; }
    public LevelManager levelManager { get; private set; }

    protected override void SetDefaults()
    {
        //get references
        uiManager = FindObjectOfType<UIManager>();
        pathFindingAStar = FindObjectOfType<PathFindingAStar>();
        levelManager = FindObjectOfType<LevelManager>();
    }
}