using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelActiverandDeactiver : MonoBehaviour
{
    public GameObject Player;
    public GameObject TilesOn;
    public GameObject TilesOff;

    void OnTriggerEnter2D(Collider2D other)
    {
        TilesOn.SetActive(true);
        TilesOff.SetActive(false);
    }
}
