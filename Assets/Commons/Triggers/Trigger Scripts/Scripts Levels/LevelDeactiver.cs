using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDeactiver : MonoBehaviour
{
    public GameObject Player;
    public GameObject TilesOff;

    void OnTriggerEnter2D(Collider2D other)
    {
        TilesOff.SetActive(false);
    }
}
