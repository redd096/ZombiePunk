using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOff : MonoBehaviour
{
    public GameObject[] turnOff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (GameObject turnOff in turnOff)
            {
                turnOff.SetActive(true);
            }
        }
    }
}
