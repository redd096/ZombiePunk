using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfEndingEnemies : MonoBehaviour
{
    private SurfEnding stuff;

    // Start is called before the first frame update
    void Start()
    {
        stuff = GameObject.FindObjectOfType<SurfEnding>();
        if (stuff != null)
            stuff.AddEnemy();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (stuff != null)
            stuff.RemoveEnemy();
    }
}
