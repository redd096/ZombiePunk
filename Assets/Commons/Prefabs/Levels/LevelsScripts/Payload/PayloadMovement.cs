using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadMovement : MonoBehaviour
{
    public float speed;

    //private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        //rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.transform.Translate(Vector3.left * speed * Time.deltaTime);
        }

        //rb.velocity = transform.right  * speed * Time.deltaTime;
    }
}
