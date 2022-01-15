using System.Collections.Generic;
using UnityEngine;
using redd096;
using NaughtyAttributes;

public class SelectWeapon : MonoBehaviour
{
    [Header("Buttons Weapons")]
    [SerializeField] bool hideButtonsIfNotUsed = true;
    [SerializeField] CustomizeButton[] buttonsInScene = default;
    [SerializeField] WeaponBASE[] weapons = default;

    [Header("Scene to Load after selected weapon")]
    [SerializeField] [Scene] string sceneToLoad = "SampleScene";

    void Start()
    {
        //create buttons
        CreateButtons();
    }

    void OnClickButton(WeaponBASE weapon)
    {
        //save in game manager
        if (GameManager.instance)
            GameManager.instance.SetWeapon(weapon);
        else
            Debug.LogWarning("There is no Game Manager in scene");

        //load next scene
        if (SceneLoader.instance)
            SceneLoader.instance.LoadScene(sceneToLoad);
        else
            Debug.LogWarning("There is no Scene Manager in scene");
    }

    #region private API

    void CreateButtons()
    {
        //for every button
        List<WeaponBASE> tempList = new List<WeaponBASE>(weapons);
        foreach (CustomizeButton customizeButton in buttonsInScene)
        {
            if (customizeButton && customizeButton.button)
            {
                //find a random element from the list and set button with this
                if (tempList.Count > 0)
                {
                    WeaponBASE random = tempList[Random.Range(0, tempList.Count)];
                    customizeButton.button.image.sprite = random.WeaponSprite;
                    customizeButton.button.onClick.AddListener(() => OnClickButton(random));

                    //if there is, set number text
                    if (customizeButton.numberText)
                        customizeButton.numberText.text = random.WeaponPrice.ToString();

                    //if there is, set description text
                    if (customizeButton.descriptionText)
                        customizeButton.descriptionText.text = random.WeaponName;

                    //remove element from the list, to not have duplicates
                    tempList.Remove(random);
                }
                //if there are not enough elements, deactive button if setted
                else if (hideButtonsIfNotUsed)
                {
                    customizeButton.button.gameObject.SetActive(false);
                }
            }
        }
    }

    #endregion
}
