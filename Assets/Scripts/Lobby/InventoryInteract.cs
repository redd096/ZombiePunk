using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;

public class InventoryInteract : BASELobbyInteract
{
    [Header("Slots of which weapon is selecting")]
    [SerializeField] WeaponButtonShop slot1 = default;
    [SerializeField] WeaponButtonShop slot2 = default;
    [SerializeField] Sprite spriteToShowWhenWeaponIsNull = default;

    [Header("UI Weapons")]
    [SerializeField] WeaponButtonShop[] weaponButtons = default;
    [Tooltip("Weapons to show always, without saves")] [SerializeField] WeaponBASE[] defaultWeapons = default;

    [Header("UI Perks")]
    [SerializeField] WeaponButtonShop[] perkButtons = default;
    [Tooltip("Perks to show always, without saves")] [SerializeField] PerkData[] defaultPerks = default;

    List<WeaponButtonShop> buttonsShop = new List<WeaponButtonShop>();
    List<ISellable> alreadyBoughtElements = new List<ISellable>();

    bool isSelectingSlot1;

    protected override void Start()
    {
        base.Start();

        //create lists
        buttonsShop = new List<WeaponButtonShop>(weaponButtons);
        buttonsShop.AddRange(perkButtons);                          //concat weapon and perks

        //initialize to save default text colors (will be saved only first time)
        foreach (WeaponButtonShop buttonShop in buttonsShop)
            if (buttonShop)
                buttonShop.Init();

        //set button event on slots
        if (slot1 && slot1.button) slot1.button.onClick.AddListener(() => OnClickSlot(true));
        if (slot2 && slot2.button) slot2.button.onClick.AddListener(() => OnClickSlot(false));
    }

    public override void Interact(InteractComponent whoInteract)
    {
        base.Interact(whoInteract);

        //set player is selecting weapon for slot 1
        OnClickSlot(true);

        //set slots sprites
        if (mainInteracting && mainInteracting.GetSavedComponent<WeaponComponent>())
        {
            WeaponBASE[] weapons = mainInteracting.GetSavedComponent<WeaponComponent>().CurrentWeapons;
            if (weapons != null)
            {
                if (weapons.Length > 0) SetSlot(weapons[0], true);
                if (weapons.Length > 1) SetSlot(weapons[1], false);
            }
        }
    }

    protected override void UpdateUI()
    {
        //load already bought weapons
        SaveClassBoughtElements saveClass = SavesManager.instance && SavesManager.instance.Load<SaveClassBoughtElements>()  != null ? SavesManager.instance.Load<SaveClassBoughtElements>() : new SaveClassBoughtElements();
        if (saveClass.BoughtWeapons == null) saveClass.BoughtWeapons = new List<WeaponBASE>();
        if (saveClass.BoughtPerks == null) saveClass.BoughtPerks = new List<PerkData>();
        alreadyBoughtElements.Clear();

        //add default weapons, then bought weapons, then null until reach weaponButtons length
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            if (i < defaultWeapons.Length)
                alreadyBoughtElements.Add(defaultWeapons[i]);
            else if (i - defaultWeapons.Length < saveClass.BoughtWeapons.Count)
                alreadyBoughtElements.Add(saveClass.BoughtWeapons[i - defaultWeapons.Length]);
            else
                alreadyBoughtElements.Add(null);
        }
        //do the same for the perks
        for (int i = 0; i < perkButtons.Length; i++)
        {
            if (i < defaultPerks.Length)
                alreadyBoughtElements.Add(defaultPerks[i]);
            else if (i - defaultPerks.Length < saveClass.BoughtPerks.Count)
                alreadyBoughtElements.Add(saveClass.BoughtPerks[i - defaultPerks.Length]);
            else
                alreadyBoughtElements.Add(null);
        }

        //check every button
        for (int i = 0; i < buttonsShop.Count; i++)
        {
            //if this button has no weapon, do not show it (ignore also if button is not setted)
            if (buttonsShop[i] == null || alreadyBoughtElements[i] == null)
            {
                if (buttonsShop[i]) buttonsShop[i].gameObject.SetActive(false);
                continue;
            }

            //else set button
            SetButton(buttonsShop[i], alreadyBoughtElements[i]);
        }
    }

    void SetButton(WeaponButtonShop buttonShop, ISellable sellable)
    {
        //be sure is active
        buttonShop.gameObject.SetActive(true);

        //set image, name and price text
        if (buttonShop.imageWeapon) buttonShop.imageWeapon.sprite = sellable.SellSprite;
        if (buttonShop.nameText) buttonShop.nameText.text = sellable.SellName;
        if (buttonShop.priceText) buttonShop.priceText.text = sellable.SellPrice.ToString();

        //set button event
        if (buttonShop.button)
        {
            buttonShop.button.onClick.RemoveAllListeners();   //remove previous listeners
            buttonShop.button.onClick.AddListener(() => OnClickButton(sellable));
        }

        //check if already equipped weapon
        bool equipped = false;
        if (sellable is WeaponBASE)
        {
            if (mainInteracting && mainInteracting.GetSavedComponent<WeaponComponent>())
            {
                //if inside list of current weapons, set is equipped
                foreach (WeaponBASE weapon in mainInteracting.GetSavedComponent<WeaponComponent>().CurrentWeapons)
                {
                    //check if is same prefab
                    if (weapon && weapon.WeaponPrefab == sellable as WeaponBASE)
                    {
                        equipped = true;
                        break;
                    }
                }
            }
        }
        //or perk
        else if (sellable is PerkData)
        {
            if (mainInteracting && mainInteracting.GetSavedComponent<PerksComponent>())
            {
                if (mainInteracting.GetSavedComponent<PerksComponent>().EquippedPerk == sellable as PerkData)
                    equipped = true;
            }
        }

        //set button active if not equipped, deactive if already equipped
        Color disabledColor = buttonShop.button ? buttonShop.button.colors.disabledColor : Color.grey;
        if (buttonShop.button) buttonShop.button.interactable = !equipped;
        if (buttonShop.nameText) buttonShop.nameText.color = equipped ? disabledColor : buttonShop.GetDefaultNameTextColor();
        if (buttonShop.priceText) buttonShop.priceText.color = equipped ? disabledColor : buttonShop.GetDefaultPriceTextColor();
    }

    void SetSlot(ISellable sellable, bool isSlot1)
    {
        //get slot
        WeaponButtonShop buttonShop = isSlot1 ? slot1 : slot2;
        if (buttonShop == null)
            return;

        //set image, name and price text
        if (buttonShop.imageWeapon) buttonShop.imageWeapon.sprite = sellable != null ? sellable.SellSprite : spriteToShowWhenWeaponIsNull;
        if (buttonShop.nameText) buttonShop.nameText.text = sellable != null ? sellable.SellName : string.Empty;
        if (buttonShop.priceText) buttonShop.priceText.text = sellable != null ? sellable.SellPrice.ToString() : string.Empty;
    }

    #region on click button

    /// <summary>
    /// When click button in the inventory, get weapon or perk
    /// </summary>
    /// <param name="sellable"></param>
    public void OnClickButton(ISellable sellable)
    {
        if (sellable != null && mainInteracting)
        {
            //pick new weapon
            if (sellable is WeaponBASE)
            {
                if (mainInteracting.GetSavedComponent<WeaponComponent>())
                {
                    WeaponComponent weaponComponent = mainInteracting.GetSavedComponent<WeaponComponent>();
                    int slot = isSelectingSlot1 ? 0 : 1;

                    //destroy weapon in the selected slot (to not drop it)
                    WeaponBASE weaponToDestroy = weaponComponent.CurrentWeapons.Length > slot && weaponComponent.CurrentWeapons[slot] ? weaponComponent.CurrentWeapons[slot] : null;
                    if (weaponToDestroy) Destroy(weaponToDestroy.gameObject);

                    //pick weapon in selected slot
                    mainInteracting.GetSavedComponent<WeaponComponent>().PickWeaponPrefab(sellable as WeaponBASE, true, isSelectingSlot1 ? 0 : 1);

                    //show in inventory slot too
                    SetSlot(sellable, isSelectingSlot1);
                }
            }
            //or perk
            else if (sellable is PerkData)
            {
                if (mainInteracting.GetSavedComponent<PerksComponent>())
                    mainInteracting.GetSavedComponent<PerksComponent>().AddPerk(sellable as PerkData);
            }

            //update UI
            UpdateUI();
        }
    }

    /// <summary>
    /// When click slot button, set which slot is selecting
    /// </summary>
    /// <param name="isSlot1"></param>
    public void OnClickSlot(bool isSlot1)
    {
        //set which slot player is selecting
        isSelectingSlot1 = isSlot1;

        //active or deactive highlight
        if (slot1 && slot1.objectToActivateWhenSelectSlot) slot1.objectToActivateWhenSelectSlot.SetActive(isSelectingSlot1);
        if (slot2 && slot2.objectToActivateWhenSelectSlot) slot2.objectToActivateWhenSelectSlot.SetActive(isSelectingSlot1 == false);
    }

    #endregion
}
