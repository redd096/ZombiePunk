using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;
using UnityEngine.UI;

public class ShopInteract : BASELobbyInteract
{
    [Header("UI")]
    [SerializeField] Text currencyText = default;
    [SerializeField] string stringCurrency = "MONEY: ";
    [SerializeField] Color colorWhenTooExpensive = Color.red;

    [Header("Weapons")]
    [SerializeField] WeaponButtonShop[] weaponButtons = default;
    [SerializeField] WeaponBASE[] weaponsToBuy = default;

    [Header("Perks")]
    [SerializeField] WeaponButtonShop[] perkButtons = default;
    [SerializeField] PerkData[] perksToBuy = default;

    [Header("Levels to Unlock")]
    [SerializeField] bool deactiveButtonsWhenLocked = false;
    [SerializeField] bool activeObjectOnButtonWhenLocked = true;
    [SerializeField] bool setDisabledColorOnButtonWhenLocked = true;
    [SerializeField] LevelStruct[] levelsToUnlock = default;

    List<WeaponButtonShop> buttonsShop = new List<WeaponButtonShop>();
    List<ISellable> elementsToBuy = new List<ISellable>();
    List<ISellable> alreadyBoughtElements = new List<ISellable>();

    protected override void Start()
    {
        base.Start();

        //create lists
        buttonsShop = new List<WeaponButtonShop>(weaponButtons);
        buttonsShop.AddRange(perkButtons);                                          //concat weapon and perks
        elementsToBuy = new List<ISellable>();                                      //add weapons and perks, but keep same length of buttonsShop
        for (int i = 0; i < weaponButtons.Length; i++)
            elementsToBuy.Add(i < weaponsToBuy.Length ? weaponsToBuy[i] : null);
        for (int i = 0; i < perksToBuy.Length; i++)
            elementsToBuy.Add(i < perksToBuy.Length ? perksToBuy[i] : null);

        //set every button in shop
        for (int i = 0; i < buttonsShop.Count; i++)
        {
            if (buttonsShop[i] && elementsToBuy[i] != null)
            {
                SetButton(buttonsShop[i], elementsToBuy[i]);
            }
        }
    }

    protected override void UpdateUI()
    {
        //check current money
        int currentMoney = mainInteracting && mainInteracting.GetSavedComponent<WalletComponent>() ? mainInteracting.GetSavedComponent<WalletComponent>().Money : 0;

        //set currency text
        if (currencyText)
            currencyText.text = stringCurrency + currentMoney.ToString();

        //load already bought weapons and perks
        SaveClassBoughtElements saveClass = SavesManager.instance ? SavesManager.instance.Load<SaveClassBoughtElements>() : null;
        alreadyBoughtElements.Clear();
        if (saveClass != null) alreadyBoughtElements = saveClass.BoughtElements;

        //load level reached
        SaveClassLevelReached saveClassLevels = SavesManager.instance ? SavesManager.instance.Load<SaveClassLevelReached>() : null;
        int reachedLevel = saveClassLevels != null ? saveClassLevels.LevelReached : 0;

        //check every button
        for (int i = 0; i < buttonsShop.Count; i++)
        {
            //if button or element to buy are not setted, do not show this button
            if (buttonsShop[i] == null || elementsToBuy[i] == null)
            {
                if (buttonsShop[i]) buttonsShop[i].gameObject.SetActive(false);
                continue;
            }

            //else be sure is active
            buttonsShop[i].gameObject.SetActive(true);

            //if button isn't unlocked (player hasn't reached the necessary level), lock it
            if (IsButtonUnlocked(buttonsShop[i].gameObject, reachedLevel) == false)
            {
                SetButtonUnlocked(buttonsShop[i], false);
                continue;
            }

            //else be sure to unlock it
            SetButtonUnlocked(buttonsShop[i], true);

            //if already bought, deactive button
            if (alreadyBoughtElements.Contains(elementsToBuy[i]))
            {
                Color disabledColor = buttonsShop[i].button ? buttonsShop[i].button.colors.disabledColor : Color.grey;
                if (buttonsShop[i].button) buttonsShop[i].button.interactable = false;
                if (buttonsShop[i].nameText) buttonsShop[i].nameText.color = disabledColor;
                if (buttonsShop[i].priceText) buttonsShop[i].priceText.color = disabledColor;
            }
            //else check price to set interactable or not
            else
            {
                bool canBuy = currentMoney >= elementsToBuy[i].SellPrice;
                if (buttonsShop[i].button) buttonsShop[i].button.interactable = canBuy;
                if (buttonsShop[i].nameText) buttonsShop[i].nameText.color = canBuy ? buttonsShop[i].GetDefaultNameTextColor() : colorWhenTooExpensive;
                if (buttonsShop[i].priceText) buttonsShop[i].priceText.color = canBuy ? buttonsShop[i].GetDefaultPriceTextColor() : colorWhenTooExpensive;
            }
        }
    }

    #region private API

    void SetButton(WeaponButtonShop buttonShop, ISellable sellable)
    {
        //initialize to save default text colors (will be saved only first time)
        if (buttonShop)
            buttonShop.Init();

        if (sellable != null)
        {
            //set image, name and price text
            if (buttonShop.imageWeapon) buttonShop.imageWeapon.sprite = sellable.SellSprite;
            if (buttonShop.nameText) buttonShop.nameText.text = sellable.SellName;
            if (buttonShop.priceText) buttonShop.priceText.text = sellable.SellPrice.ToString();

            //set button event
            if (buttonShop.button)
            {
                buttonShop.button.onClick.AddListener(() => OnClickButton(sellable));
            }
        }
    }

    bool IsButtonUnlocked(GameObject buttonToCheck, int reachedLevel)
    {
        if (buttonToCheck == null)
            return false;

        //find button in the list
        foreach (LevelStruct level in levelsToUnlock)
        {
            //return if reached necessary level
            if (buttonToCheck == level.ObjectToActivate)
                return reachedLevel >= level.LevelToComplete;
        }

        //if not in the list, is unlocked by default
        return true;
    }

    void SetButtonUnlocked(WeaponButtonShop buttonShop, bool isUnlocked)
    {
        //set not interactable when locked
        if (buttonShop.button) 
            buttonShop.button.interactable = isUnlocked;

        //deactive buttons - deactive it when is locked, reactive when unlocked
        if (deactiveButtonsWhenLocked)
        {
            buttonShop.gameObject.SetActive(isUnlocked);
        }
        //active object - active it when is locked, deactive when unlocked
        if (activeObjectOnButtonWhenLocked)
        {
            if (buttonShop.objectToActivateWhenButtonIsLocked) buttonShop.objectToActivateWhenButtonIsLocked.SetActive(!isUnlocked);
        }
        //set disabled color - set it when locked, reset when unlocked
        if (setDisabledColorOnButtonWhenLocked)
        {
            Color disabledColor = buttonShop.button ? buttonShop.button.colors.disabledColor : Color.grey;
            if (buttonShop.nameText) buttonShop.nameText.color = isUnlocked ? buttonShop.GetDefaultNameTextColor() : disabledColor;
            if (buttonShop.priceText) buttonShop.priceText.color = isUnlocked ? buttonShop.GetDefaultPriceTextColor() : disabledColor;
        }
    }

    #endregion

    #region function for UI Buttons

    public void OnClickButton(ISellable sellable)
    {
        //be sure there is a weapon
        if (sellable != null && mainInteracting)
        {
            //be sure player has enough money, and this weapons is not already bought (teorically is not possible, because the button is deactivated when update UI)
            int currentMoney = mainInteracting.GetSavedComponent<WalletComponent>() ? mainInteracting.GetSavedComponent<WalletComponent>().Money : 0;
            if (currentMoney >= sellable.SellPrice && alreadyBoughtElements.Contains(sellable) == false)
            {
                //add to already bought weapons
                alreadyBoughtElements.Add(sellable);

                //save
                SaveClassBoughtElements saveClass = SavesManager.instance && SavesManager.instance.Load<SaveClassBoughtElements>() != null ? SavesManager.instance.Load<SaveClassBoughtElements>() : new SaveClassBoughtElements();
                saveClass.BoughtElements = alreadyBoughtElements;
                if (SavesManager.instance) SavesManager.instance.Save(saveClass);

                //remove money
                if (mainInteracting.GetSavedComponent<WalletComponent>())
                    mainInteracting.GetSavedComponent<WalletComponent>().Money -= sellable.SellPrice;

                if (sellable is WeaponBASE)
                {
                    //if current weapons are full, destroy current equipped (to not drop it)
                    if (mainInteracting.GetSavedComponent<WeaponComponent>() && mainInteracting.GetSavedComponent<WeaponComponent>().IsFull())
                        Destroy(mainInteracting.GetSavedComponent<WeaponComponent>().CurrentWeapon.gameObject);

                    //pick weapon
                    if (mainInteracting.GetSavedComponent<WeaponComponent>())
                        mainInteracting.GetSavedComponent<WeaponComponent>().PickWeaponPrefab(sellable as WeaponBASE);
                }
                else if (sellable is PerkData)
                {
                    //else pick perk
                    if (mainInteracting.GetSavedComponent<PerksComponent>())
                        mainInteracting.GetSavedComponent<PerksComponent>().AddPerk(sellable as PerkData);
                }

                //update UI
                UpdateUI();
            }
        }
    }

    /// <summary>
    /// Delete saved bought things
    /// </summary>
    public void ClearAllSaves()
    {
        //clear weapons and perks
        if (SavesManager.instance)
        {
            SavesManager.instance.ClearSave<SaveClassBoughtElements>();
            SavesManager.instance.ClearSave<SaveClassLevelReached>();
        }

        //update UI
        UpdateUI();
    }

    /// <summary>
    /// Save every thing unlocked
    /// </summary>
    public void UnlockAllSaves()
    {
        //load already bought weapons
        SaveClassBoughtElements saveClass = SavesManager.instance && SavesManager.instance.Load<SaveClassBoughtElements>() != null ? SavesManager.instance.Load<SaveClassBoughtElements>() : new SaveClassBoughtElements();
        alreadyBoughtElements = (saveClass.BoughtElements != null) ? saveClass.BoughtElements : new List<ISellable>();

        //get every element
        foreach (ISellable element in elementsToBuy)
            if (element != null && alreadyBoughtElements.Contains(element) == false)
                alreadyBoughtElements.Add(element);

        //save
        saveClass.BoughtElements = alreadyBoughtElements;
        if (SavesManager.instance) SavesManager.instance.Save(saveClass);



        //load level reached
        SaveClassLevelReached levelSaveClass = SavesManager.instance && SavesManager.instance.Load<SaveClassLevelReached>() != null ? SavesManager.instance.Load<SaveClassLevelReached>() : new SaveClassLevelReached();
        int reachedLevel = levelSaveClass.LevelReached;

        //find last possible level to reach
        foreach (LevelStruct level in levelsToUnlock)
        {
            if (level.LevelToComplete > reachedLevel)
                reachedLevel = level.LevelToComplete;
        }

        //save level reached
        levelSaveClass.LevelReached = reachedLevel;
        if (SavesManager.instance) SavesManager.instance.Save(levelSaveClass);



        //update UI
        UpdateUI();
    }

    #endregion
}
