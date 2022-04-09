using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerLevelTrigger : MonoBehaviour
{
    public GameObject levelEnter, levelPlay, game , exit, Spawn, objectiveon, objectiveoff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (game) game.SetActive(true);
            objectiveon.SetActive(true);
            objectiveoff.SetActive(false);
            print("DioMadonna");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            Destroy(Spawn);
            
            if (levelPlay) levelPlay.SetActive(true);

            if (levelEnter) levelEnter.SetActive(false);
            if (exit) exit.SetActive(true);
            Destroy(gameObject);
        }
    }
}
