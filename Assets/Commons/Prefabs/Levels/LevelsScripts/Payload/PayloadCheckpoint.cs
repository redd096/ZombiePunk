using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadCheckpoint : MonoBehaviour
{
    public GameObject toTurnOn, toTurnOff;

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
            toTurnOn.SetActive(true);
            Destroy(toTurnOff);
        }
    }
}
