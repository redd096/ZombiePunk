﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using redd096.GameTopDown2D;

public class ShowWeapon : MonoBehaviour
{
    public Sprite emptyImage;

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
        if (gameObject.name == "WeaponMain" && weapons)
        {
            current = weapons.IndexEquippedWeapon;
            if (weapons.CurrentWeapons[current]) image.sprite = weapons.CurrentWeapons[current].GetComponent<WeaponRange>().WeaponSpriteTopUI;
        }

        if(gameObject.name == "WeaponSec" && weapons && weapons.CurrentWeapons.Length >= 2)
        {
            current = weapons.IndexEquippedWeapon;

            if (weapons.CurrentWeapons[1] == null)
            {
                image.sprite = emptyImage;
            }

            if(current == 0)
            {
                if (weapons.CurrentWeapons[1]) image.sprite = weapons.CurrentWeapons[1].GetComponent<WeaponRange>().WeaponSpriteTopUI;
            }
            else
            {
                if (weapons.CurrentWeapons[0]) image.sprite = weapons.CurrentWeapons[0].GetComponent<WeaponRange>().WeaponSpriteTopUI;
            }
        }
    }
}
