using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeObjective : MonoBehaviour
{
    
    public GameObject newobjective,oldobjective;

    private KillAllEnemies room1;
    // Start is called before the first frame update
    void Start()
    {
        room1 = GetComponent<KillAllEnemies>();
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
    
    void Refresh()
    {
        //if (room1.exit == true)
        //{
        //   print("sdfg");
        //}
        
    }
}
