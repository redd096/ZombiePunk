using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096.Attributes;


namespace redd096.GameTopDown2D
{
    public class MapInteract : MonoBehaviour, IInteractable
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Interact(InteractComponent whoInteract)
        {
            SceneLoader.instance.MapPause();
        }
    }
}

