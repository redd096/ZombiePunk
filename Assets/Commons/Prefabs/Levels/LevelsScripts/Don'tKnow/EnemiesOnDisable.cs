using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesOnDisable : MonoBehaviour
{
    public GameObject toActivate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDisable()
    {
        if (toActivate)
            toActivate.SetActive(true);
    }
}
