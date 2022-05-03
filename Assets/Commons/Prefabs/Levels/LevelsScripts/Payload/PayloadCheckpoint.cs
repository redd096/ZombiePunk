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
        //do only if there is still a payload in scene
        if (payload == null)
            return;

        if (gameObject.transform.position.x <= payload.transform.position.x)
        {
            if (toTurnOn) toTurnOn.SetActive(true);
            if (toTurnOff) Destroy(toTurnOff);            
        }
    }
}
