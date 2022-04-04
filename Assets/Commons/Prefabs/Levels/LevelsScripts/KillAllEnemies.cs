using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class KillAllEnemies : MonoBehaviour
    {
        public GameObject play, exit;

        private int open;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            Debug.Log(open);

            if(open > 0)
            {
                play.SetActive(true);
                exit.SetActive(false);
            }
            else
            {
                play.SetActive(false);
                exit.SetActive(true);
            }
        }

        public void AddEnemy()
        {
            open += 1;
        }

        public void RemoveEnemy()
        {
            open -= 1;
        }
    }
