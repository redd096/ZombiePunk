using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAdvice : MonoBehaviour
{
    public GameObject notification;
    private WalletComponent wallet;
    // Start is called before the first frame update
    void Start()
    {
        wallet = GameObject.Find("Player").GetComponent<WalletComponent>();
        StartCoroutine("Coso");
    }

    // Update is called once per frame
    void Update()
    {
      // wallet = GameObject.Find("Player").GetComponent<WalletComponent>();
      //
      // if (wallet.Money >= 40)
      // {
      //     notification.SetActive(true);
      //     print("Checavolo");
      // }
    }
    void PincoPallo()
    {
        if (wallet.Money >= 40)
        {
            notification.SetActive(true);
            print("Checavolo");
        }
    }

    IEnumerator Coso()
    {
        yield return new WaitForSeconds(0.5f);
        PincoPallo();
    }
}
