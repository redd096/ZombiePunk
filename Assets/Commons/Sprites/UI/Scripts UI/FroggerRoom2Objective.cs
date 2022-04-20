using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FroggerRoom2Objective : MonoBehaviour
{
    public TMP_Text billboardsnumber;
    private int upnum;
    private FroggerDoor counter;

    // Start is called before the first frame update
    void Start()
    {
        counter = GetComponent<FroggerDoor>();
    }

    // Update is called once per frame
    void Update()
    {
        counting();
    }
    void counting()
    {
        if (counter.number <= 4)
        {
            upnum++;
            billboardsnumber.text = upnum.ToString();
        }
    }

}
