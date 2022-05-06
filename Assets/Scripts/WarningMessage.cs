using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningMessage : MonoBehaviour
{
    public GameObject warning;

    // Start is called before the first frame update

    void OnTriggerEnter2D(Collider2D collider)
    {

         warning.SetActive(true);


    }
    void OnTriggerExit2D(Collider2D collider)
    {
        warning.SetActive(false);
    }
}
