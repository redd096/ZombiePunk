using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadCheckpoint : MonoBehaviour
{
    public GameObject toTurnOn1,toTurnOn2, toTurnOff;

    private GameObject payload;

    // Start is called before the first frame update
    void Start()
    {
        payload = GameObject.Find("Payload");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.x <= payload.transform.position.x)
        {
            toTurnOn1.SetActive(true);
            toTurnOn2.SetActive(true);
            Destroy(toTurnOff);
        }
    }
}
