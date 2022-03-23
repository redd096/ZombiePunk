using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using redd096.GameTopDown2D;

public class ShowWeapon : MonoBehaviour
{
    private Image image;
    private AdvancedWeaponComponent weapons;
    private int current;

    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        weapons = GameObject.Find("Player").GetComponent<AdvancedWeaponComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "WeaponMain")
        {
            current = weapons.IndexEquippedWeapon;
            image.sprite = weapons.CurrentWeapons[current].GetComponent<WeaponRange>().WeaponSprite;
        }
    }
}
