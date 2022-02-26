using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerLevelTrigger : MonoBehaviour
{
    public GameObject levelEnter, levelPlay, game , exit, Spawn;

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
            game.SetActive(true);
            print("DioMadonna");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            Destroy(Spawn);
            
            levelPlay.SetActive(true);

            levelEnter.SetActive(false);
            exit.SetActive(true);
            Destroy(gameObject);
        }
    }
}
