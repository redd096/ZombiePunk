using System.Collections.Generic;
using UnityEngine;
using redd096;
using UnityEngine.UI;
using NaughtyAttributes;

#region structs

public struct CustomButtonStruct
{
    public Button button;
    public CustomizeData customization;

    public CustomButtonStruct(Button button, CustomizeData customization)
    {
        this.button = button;
        this.customization = customization;
    }
}

#endregion

public class CustomizeWeapon : MonoBehaviour
{
    [Header("Buttons Customizations")]
    [SerializeField] bool hideButtonsIfNotUsed = true;
    [SerializeField] GameObject[] buttonsInScene = default;
    [SerializeField] CustomizeData[] customizations = default;

    [Header("Inventory")]
    [SerializeField] int maxWeight = 10;
    [SerializeField] GameObject prefabInventoryElement = default;
    [SerializeField] Transform inventoryParent = default;
    [SerializeField] Transform trashParent = default;
    [SerializeField] Text weightText = default;

    [Header("Scene to Load after selected weapon")]
    [SerializeField] [Scene] string sceneToLoad = "SampleScene";

    [Button("Update Clickable Buttons", EButtonEnableMode.Playmode)]
    void UpdateClickableButtons() { ShowTotalWeight(); SetClickableButtons(); }

    /// <summary>
    /// save which button is showing which customization
    /// </summary>
    List<CustomButtonStruct> showedCustomizations = new List<CustomButtonStruct>();
    /// <summary>
    /// customizations in inventory, to save for next scenes
    /// </summary>
    List<CustomizeData> inventoryCustomizations = new List<CustomizeData>();
    /// <summary>
    /// customizations in the trash. Can be replaced in inventory. When exit from this scene will be destroyed
    /// </summary>
    List<CustomizeData> trashCustomizations = new List<CustomizeData>();
    /// <summary>
    /// weight of every element in inventory
    /// </summary>
    int inventoryTotalWeight = 0;

    void Start()
    {
        //reset vars
        showedCustomizations.Clear();
        inventoryCustomizations.Clear();
        trashCustomizations.Clear();
        inventoryTotalWeight = 0;
        if (inventoryParent)
        {
            foreach (Transform child in inventoryParent)
                Destroy(child.gameObject);
        }
        if (trashParent)
        {
            foreach (Transform child in trashParent)
                Destroy(child.gameObject);
        }

        //create buttons
        CreateButtons();

        //get current customizations
        if (GameManager.instance)
            inventoryCustomizations.AddRange(GameManager.instance.GetCustomizations());

        //show inventory and total weight
        if (inventoryCustomizations != null)
        {
            ShowInventoryAndCalculateTotalWeight();
            ShowTotalWeight();
        }

        //set which button is clickable (based on weight)
        SetClickableButtons();
    }

    #region private API

    void CreateButtons()
    {
        //for every button
        List<CustomizeData> tempCustomizations = new List<CustomizeData>(customizations);
        foreach (GameObject element in buttonsInScene)
        {
            CustomizeButton customizeButton = element.GetComponentInChildren<CustomizeButton>();
            if (customizeButton && customizeButton.button)
            {
                //find a random customization and set button with this
                if (tempCustomizations.Count > 0)
                {
                    CustomizeData random = tempCustomizations[Random.Range(0, tempCustomizations.Count)];
                    customizeButton.button.image.sprite = random.Sprite;
                    customizeButton.button.onClick.AddListener(() => AddCustomization(random));

                    //if there is text, set with customization weight
                    if (customizeButton.numberText)
                        customizeButton.numberText.text = random.Weight.ToString();

                    //if there is a description, set with customization description
                    if (customizeButton.descriptionText)
                        customizeButton.descriptionText.text = random.Description;

                    //save which customization is using this button
                    showedCustomizations.Add(new CustomButtonStruct(customizeButton.button, random));

                    //remove customization from the list, to not have duplicates
                    tempCustomizations.Remove(random);
                }
                //if there are not enough customizations, deactive button if setted
                else if (hideButtonsIfNotUsed)
                {
                    customizeButton.button.gameObject.SetActive(false);
                }
            }
        }
    }

    void ShowInventoryAndCalculateTotalWeight()
    {
        //instantiate every button and calculate weight
        foreach(CustomizeData customization in inventoryCustomizations)
        {
            //calculate weight
            inventoryTotalWeight += customization.Weight;

            //instantiate button
            if (prefabInventoryElement && inventoryParent)
            {
                GameObject go = Instantiate(prefabInventoryElement, inventoryParent, false);

                //set button sprite, text and function
                SetButton(go, customization, RemoveCustomization);
            }
        }
    }

    void ShowTotalWeight()
    {
        //set text with total weight
        if (weightText)
            weightText.text = $"{inventoryTotalWeight}/{maxWeight}";
    }

    void SetClickableButtons()
    {
        //foreach button, check weight of customization
        foreach(CustomButtonStruct customButton in showedCustomizations)
        {
            if(customButton.customization)
            {
                //if exceed max weight, deactive
                if(inventoryTotalWeight + customButton.customization.Weight > maxWeight)
                {
                    if (customButton.button) customButton.button.interactable = false;
                }
                //else, active it
                else
                {
                    if (customButton.button) customButton.button.interactable = true;
                }
            }
        }
    }

    void SetButton(GameObject go, CustomizeData customization, System.Action<GameObject, CustomizeData> func)
    {
        CustomizeButton customizeButton = go.GetComponentInChildren<CustomizeButton>();

        //set button sprite and function
        if (customizeButton && customizeButton.button)
        {
            customizeButton.button.image.sprite = customization.Sprite;
            customizeButton.button.onClick.AddListener(() => func(go, customization));
        }

        //set text with customization weight
        if (customizeButton && customizeButton.numberText)
            customizeButton.numberText.text = customization.Weight.ToString();

        //set text with customization description
        if (customizeButton && customizeButton.descriptionText)
            customizeButton.descriptionText.text = customization.Description;
    }

    #endregion

    #region buttons

    void AddCustomization(CustomizeData customization)
    {
        //do only if is possible (weight do not exceed max weight)
        if (inventoryTotalWeight + customization.Weight > maxWeight)
            return;

        //add to inventory
        inventoryCustomizations.Add(customization);

        //save in game manager
        if (GameManager.instance)
            GameManager.instance.SetCustomizations(inventoryCustomizations.ToArray());
        else
            Debug.LogWarning("There is no Game Manager in scene");

        //load next scene
        if (SceneLoader.instance)
            SceneLoader.instance.LoadScene(sceneToLoad);
        else
            Debug.LogWarning("There is no Scene Manager in scene");
    }

    void RemoveCustomization(GameObject element, CustomizeData customization)
    {
        if (inventoryCustomizations.Contains(customization))
        {
            //remove customization from inventory and remove weight
            inventoryCustomizations.Remove(customization);
            inventoryTotalWeight -= customization.Weight;

            //add to trash
            trashCustomizations.Add(customization);

            //instantiate button in the trash
            if (prefabInventoryElement && trashParent)
            {
                GameObject go = Instantiate(prefabInventoryElement, trashParent, false);

                //set button sprite, text and function
                SetButton(go, customization, PickCustomizationFromTrash);
            }
        }

        //remove button from scene and update weight text
        if(element) Destroy(element);
        ShowTotalWeight();

        //recheck which button is clickable (based on weight)
        SetClickableButtons();
    }

    void PickCustomizationFromTrash(GameObject element, CustomizeData customization)
    {
        if (trashCustomizations.Contains(customization))
        {
            //remove customization from trash
            trashCustomizations.Remove(customization);

            //add to inventory and add weight
            inventoryCustomizations.Add(customization);
            inventoryTotalWeight += customization.Weight;

            //instantiate button in the inventory
            if (prefabInventoryElement && inventoryParent)
            {
                GameObject go = Instantiate(prefabInventoryElement, inventoryParent, false);

                //set button sprite, text and function
                SetButton(go, customization, RemoveCustomization);
            }
        }

        //remove button from scene and update weight text
        if (element) Destroy(element);
        ShowTotalWeight();

        //recheck which button is clickable (based on weight)
        SetClickableButtons();
    }

    #endregion
}
