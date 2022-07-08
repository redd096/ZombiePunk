using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;

public class TriggerDie : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Player"))
        //{
        //    life.Die(null);
        //}

        HealthComponent health = collision.GetComponentInParent<HealthComponent>();
        if (health != null)
        {
            health.Die(null);
        }
    }
}
