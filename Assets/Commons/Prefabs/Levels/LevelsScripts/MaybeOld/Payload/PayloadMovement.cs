using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadMovement : MonoBehaviour
{
    public float speed;

    //private Rigidbody2D rb;
    private Collider2D player;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {

    }

     //Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (player)
        {
            //rb.velocity = transform.right * speed;
            rb.velocity = new Vector2(speed, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision;
            rb.velocity = new Vector2(speed, 0);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null;
            //rb.velocity = transform.right * 0;
            rb.velocity = new Vector2(0, 0);
        }
    }
}
