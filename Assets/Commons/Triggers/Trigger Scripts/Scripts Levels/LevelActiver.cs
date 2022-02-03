using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelActiver : MonoBehaviour
{
    public GameObject Player;
    public GameObject TilesOn;

    void OnTriggerEnter2D(Collider2D other)
    {
        TilesOn.SetActive(true);
    }

        
}
