using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffObjective : MonoBehaviour
{
    public GameObject TurnOffObj;

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
            TurnOffObj.SetActive(false);

        }
    }
}
