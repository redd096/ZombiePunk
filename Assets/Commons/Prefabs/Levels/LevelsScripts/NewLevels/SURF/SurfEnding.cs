using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfEnding : MonoBehaviour
{
    private GameObject player;
    private int open;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (open <= 0)
        {
            player.transform.Translate(Vector3.right * 5 * Time.deltaTime);
        }
    }

    public void AddEnemy()
    {
        open += 1;
    }

    public void RemoveEnemy()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine("ActuallyRemoveEnemy");
    }

    IEnumerator ActuallyRemoveEnemy()
    {
        yield return new WaitForSeconds(8f);
        open -= 1;
    }
}
