using redd096.GameTopDown2D;
using redd096.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectiveCompleted : MonoBehaviour
{
    //public Animation completedanim;
    public GameObject test;
    public GameObject coso;

    private redd096.GameTopDown2D.ExitInteractable exit; 
    
    // Start is called before the first frame update
    void Start()
    {
        exit = GetComponent<ExitInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void FinishedObjective()
    {
        if (exit.IsOpen)
        {
            coso.SetActive(false);
            test.SetActive(true);


        }
    }
}
