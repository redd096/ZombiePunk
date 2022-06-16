using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;

public class TriggerDie : MonoBehaviour
{
    private HealthComponent life;

    // Start is called before the first frame update
    void Start()
    {
        life = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            life.Die(null);
        }
    }
}
