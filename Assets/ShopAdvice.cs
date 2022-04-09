using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using redd096.GameTopDown2D;
using redd096;

namespace redd096
{
    [AddComponentMenu("redd096/MonoBehaviours/UI Manager")]

    public class ShopAdvice : MonoBehaviour
    {
        public GameObject notification;
        private GameObject Player;
        private WalletComponent wallet;

        
        void Start()
        {
            
            

           // if (currentMoney > 40)
            {
                print("Funziono");
                notification.SetActive(true);
                
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
