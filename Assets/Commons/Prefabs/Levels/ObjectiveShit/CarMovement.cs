using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed;
    public float final;

    private Vector3 ogPos;


    // Start is called before the first frame update
    void Start()
    {
        ogPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        print(ogPos);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (gameObject.transform.position.x <= final)
        {
            print(ogPos);
            gameObject.transform.position = ogPos;
        }
    }
}
