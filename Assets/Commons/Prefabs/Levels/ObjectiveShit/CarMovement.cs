using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed;
    public float time;

    private Vector3 ogPos;
    private float ogTime;


    // Start is called before the first frame update
    void Start()
    {
        ogPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        ogTime = time;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(Vector3.left * speed * Time.deltaTime);
        time -= Time.deltaTime;

        if (time <= 0)
        {
            gameObject.transform.position = ogPos;
            time = ogTime;
        }
    }
}
