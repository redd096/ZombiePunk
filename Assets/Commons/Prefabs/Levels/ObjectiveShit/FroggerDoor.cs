using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerDoor : MonoBehaviour
{
    public int number;
    public GameObject levelPlay, levelExit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (number <= 0)
        {
            levelExit.SetActive(true);
            levelPlay.SetActive(false);
        }
    }
}
