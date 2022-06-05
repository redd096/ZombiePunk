using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B2_Timer : MonoBehaviour
{
    public float time;
    public GameObject entrance, exit;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = time;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1 * Time.deltaTime;

        if(timer <= 0)
        {
            entrance.SetActive(false);
            exit.SetActive(true);
        }
    }
}
