using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;
using UnityEngine.UI;

public class LifeTest : MonoBehaviour
{
    public HealthComponent life;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = (life.CurrentHealth) / 100;
    }
}
