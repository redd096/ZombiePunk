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
                if (play != null) play.SetActive(true);
                if (exit != null) exit.SetActive(false);
            }
            else
            {
                if (play != null) play.SetActive(false);
                if (exit != null) exit.SetActive(true);
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
             yield return new WaitForSeconds(2.2f);
             open -= 1; 
        }
    }
