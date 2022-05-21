using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class KillAllEnemies : MonoBehaviour
    {
        public GameObject[] play, exit;

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

            foreach (GameObject play in play)
            {
                play.SetActive(true);
            }

            foreach (GameObject exit in exit)
            {
                exit.SetActive(false);
            }
            }
            else
            {
            foreach (GameObject play in play)
            {
                play.SetActive(false);
            }

            foreach (GameObject exit in exit)
            {
                exit.SetActive(true);
            }
        }
        }

        public void AddEnemy()
        {
            open += 1;
        }

        public void RemoveEnemy()
        {
            //open -= 1;
            if (gameObject.activeInHierarchy)
                StartCoroutine("ActuallyRemoveEnemy");
        }

        IEnumerator ActuallyRemoveEnemy()
        {
             yield return new WaitForSeconds(1.2f);
             open -= 1; 
        }
    }
