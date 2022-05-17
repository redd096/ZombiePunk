using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class SurfParticles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (ParticlesManager.instance.ParticlesParent != null)
        {
            ParticlesManager.instance.ParticlesParent.Translate(Vector2.left * 10 * Time.deltaTime);
        }
        if (InstantiateGameObjectManager.instance.Parent != null)
        {
            InstantiateGameObjectManager.instance.Parent.Translate(Vector2.left * 10 * Time.deltaTime);
        }
    }

}
