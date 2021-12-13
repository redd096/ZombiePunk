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
    [SerializeField] Text weightText = default;

    [Header("Scene to Load after selected weapon")]
    [SerializeField] [Scene] string sceneToLoad = "SampleScene";

    [Button("Update Clickable Buttons", EButtonEnableMode.Playmode)]
    void UpdateClickableButtons() => SetClickableButtons();

    /// <summary>
    /// save which button is showing which customization
    /// </summary>
    List<CustomButtonStruct> showedCustomizations = new List<CustomButtonStruct>();
    /// <summary>
    /// customizations in inventory, to save for next scenes
    /// </summary>
    List<CustomizeData> currentCustomizations = new List<CustomizeData>();
    /// <summary>
    /// weight of every element in inventory
    /// </summary>
    int currentTotalWeight = 0;

    void Start()
    {
        showedCustomizations.Clear();
        currentCustomizations.Clear();
        currentTotalWeight = 0;

        //create buttons
        CreateButtons();

        //get current customizations
        if (GameManager.instance)
            currentCustomizations.AddRange(GameManager.instance.GetCustomizations());

        //show inventory and total weight
        if (currentCustomizations != null)
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
            Button button = element.GetComponentInChildren<Button>();
            if (button)
            {
                //find a random customization and set button with this
                if (tempCustomizations.Count > 0)
                {
                    CustomizeData random = tempCustomizations[Random.Range(0, tempCustomizations.Count)];
                    button.image.sprite = random.Sprite;
                    button.onClick.AddListener(() => AddCustomization(random));

                    //if there is text, set with customization weight
                    Text textWeight = element.GetComponentInChildren<Text>();
                    if (textWeight)
                        textWeight.text = random.Weight.ToString();

                    //save which customization is using this button
                    showedCustomizations.Add(new CustomButtonStruct(button, random));

                    //remove customization from the list, to not have duplicates
                    tempCustomizations.Remove(random);
                }
                //if there are not enough customizations, deactive button if setted
                else if (hideButtonsIfNotUsed)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

    void ShowInventoryAndCalculateTotalWeight()
    {
        //instantiate every button and calculate weight
        foreach(CustomizeData customization in currentCustomizations)
        {
            currentTotalWeight += customization.Weight;

            if (prefabInventoryElement == null || inventoryParent == null)
                continue;

            GameObject go = Instantiate(prefabInventoryElement, inventoryParent, false);

            //set button sprite and function
            Button button = go.GetComponentInChildren<Button>();
            if(button)
            {
                button.image.sprite = customization.Sprite;
                button.onClick.AddListener(() => RemoveCustomization(go, customization));
            }

            //set text it with customization weight
            Text text = go.GetComponentInChildren<Text>();
            if (text)
                text.text = customization.Weight.ToString();
        }
    }

    void ShowTotalWeight()
    {
        //set text with total weight
        if (weightText)
            weightText.text = $"{currentTotalWeight}/{maxWeight}";
    }

    void SetClickableButtons()
    {
        //foreach button, check weight of customization
        foreach(CustomButtonStruct customButton in showedCustomizations)
        {
            if(customButton.customization)
            {
                //if exceed max weight, deactive
                if(currentTotalWeight + customButton.customization.Weight > maxWeight)
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

    #endregion

    #region buttons

    void AddCustomization(CustomizeData customization)
    {
        //do only if is possible (weight do not exceed max weight)
        if (currentTotalWeight + customization.Weight > maxWeight)
            return;

        //add to inventory
        currentCustomizations.Add(customization);

        //save in game manager
        if (GameManager.instance)
            GameManager.instance.SetCustomizations(currentCustomizations.ToArray());
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
        //remove customization from inventory and remove weight
        currentCustomizations.Remove(customization);
        currentTotalWeight -= customization.Weight;

        //remove button from scene and update weight text
        Destroy(element);
        ShowTotalWeight();

        //recheck which button is clickable (based on weight)
        SetClickableButtons();
    }

    #endregion
}
