using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveObjective : MonoBehaviour
{
    public GameObject ActivateObjective;

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
            ActivateObjective.SetActive(true);

        }
    }
}
