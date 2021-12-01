using System.Collections.Generic;
using UnityEngine;
using redd096;
using UnityEngine.UI;
using NaughtyAttributes;

public class TempSelectWeapon : MonoBehaviour
{
    [Header("Buttons in scene")]
    [SerializeField] bool hideButtonsIfNotUsed = true;
    [SerializeField] Button[] buttonsInScene = default;

    [Header("Prefabs of every weapon possible")]
    [SerializeField] WeaponBASE[] weaponsPrefabs = default;

    [Header("Scene to Load after selected weapon")]
    [SerializeField] [Scene] string sceneToLoad = "SampleScene";

    void Start()
    {
        //for every button
        List<WeaponBASE> weapons = new List<WeaponBASE>(weaponsPrefabs);
        for(int i = 0; i < buttonsInScene.Length; i++)
        {
            if (buttonsInScene[i])
            {
                if (weapons.Count > 0)
                {
                    //find a random weapon and set button with this
                    WeaponBASE randomWeapon = weapons[Random.Range(0, weapons.Count)];
                    buttonsInScene[i].image.sprite = randomWeapon.WeaponSprite;
                    buttonsInScene[i].onClick.AddListener(() => SetWeapon(randomWeapon));

                    //remove weapon from the list, to not have duplicates
                    weapons.Remove(randomWeapon);
                }
                //if there are not enough weapons, deactive button if setted
                else if(hideButtonsIfNotUsed)
                {
                    buttonsInScene[i].gameObject.SetActive(false);
                }
            }
        }
    }

    //button function
    void SetWeapon(WeaponBASE weapon)
    {
        //save in game manager next weapon
        if (GameManager.instance)
        {
            GameManager.instance.SetWeapon(weapon);
        }
        else
        {
            Debug.LogWarning("There is no Game Manager in scene");
        }

        //load next scene
        if (SceneLoader.instance)
            SceneLoader.instance.LoadScene(sceneToLoad);
    }
}
