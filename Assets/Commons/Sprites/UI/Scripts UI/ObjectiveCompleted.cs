using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectiveCompleted : MonoBehaviour
{
    //public Animation completedanim;
    public GameObject Test;

    private redd096.GameTopDown2D.ExitInteractable exit; 
    
    // Start is called before the first frame update
    void Start()
    {
        exit = GetComponent<redd096.GameTopDown2D.ExitInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FinishedObjective()
    {
        if (exit.IsOpen)
        {
            Test.SetActive(true);
        }
    }
}
