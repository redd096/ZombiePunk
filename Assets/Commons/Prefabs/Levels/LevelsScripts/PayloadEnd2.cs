using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadEnd2 : MonoBehaviour
{
    public GameObject door, fakeDoor;

    private GameObject payload;
    private PayloadMovement script;

    // Start is called before the first frame update
    void Start()
    {
        payload = GameObject.Find("Payload");
        script = GameObject.Find("Payload").GetComponent<PayloadMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(payload.transform.position.x >= gameObject.transform.position.x)
        {
            Destroy(script);
            Destroy(fakeDoor);
            door.SetActive(true);
        }
    }
}