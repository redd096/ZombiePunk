using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;
using UnityEngine.UI;

public class InventoryInteract : BASELobbyInteract
{
    [Header("UI")]
    [SerializeField] WeaponButtonShop[] weaponButtons = default;
    [Tooltip("Weapons to show always, without saves")] [SerializeField] WeaponBASE[] defaultWeapons = default;

    protected override void UpdateUI()
    {
        //load already bought weapons
        SaveClassBoughtWeapons saveClass = GameManager.instance ? GameManager.instance.Load<SaveClassBoughtWeapons>() : null;
        List<WeaponBASE> alreadyBoughtWeapons = new List<WeaponBASE>(defaultWeapons);                                       //add default weapons to show always
        if (saveClass != null && saveClass.BoughtWeapons != null) alreadyBoughtWeapons.AddRange(saveClass.BoughtWeapons);   //add bought weapons

        //check every button
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            //initialize to save default text colors (will be saved only first time)
            if (weaponButtons[i]) weaponButtons[i].Init();

            //if this button has no weapon, do not show it (ignore also if button is not setted)
            if (i >= alreadyBoughtWeapons.Count || alreadyBoughtWeapons[i] == null || weaponButtons[i] == null)
            {
                if (weaponButtons[i]) weaponButtons[i].gameObject.SetActive(false);
                continue;
            }

            //else be sure is active
            if (weaponButtons[i]) weaponButtons[i].gameObject.SetActive(true);

            //set name and price text
            if (weaponButtons[i].nameText) weaponButtons[i].nameText.text = alreadyBoughtWeapons[i].WeaponName;
            if (weaponButtons[i].priceText) weaponButtons[i].priceText.text = alreadyBoughtWeapons[i].WeaponPrice.ToString();

            //set button event and image
            if (weaponButtons[i].button)
            {
                weaponButtons[i].button.GetComponent<Image>().sprite = alreadyBoughtWeapons[i].WeaponSprite;
                WeaponBASE weaponPrefab = alreadyBoughtWeapons[i];
                weaponButtons[i].button.onClick.RemoveAllListeners();   //remove previous listeners
                weaponButtons[i].button.onClick.AddListener(() => OnClickButton(weaponPrefab));
            }

            //check if already equipped
            if (mainInteracting && mainInteracting.GetSavedComponent<WeaponComponent>())
            {
                //if inside list of current weapons, set is equipped
                bool equipped = false;
                foreach (WeaponBASE weapon in mainInteracting.GetSavedComponent<WeaponComponent>().CurrentWeapons)
                {
                    //check if is same prefab
                    if (weapon && weapon.WeaponPrefab == alreadyBoughtWeapons[i])
                    {
                        equipped = true;
                        break;
                    }
                }

                //set button active if not equipped, deactive if already equipped
                Color disabledColor = weaponButtons[i].button ? weaponButtons[i].button.colors.disabledColor : Color.grey;
                if (weaponButtons[i].button) weaponButtons[i].button.interactable = !equipped;
                if (weaponButtons[i].nameText) weaponButtons[i].nameText.color = equipped ? disabledColor : weaponButtons[i].GetDefaultNameTextColor();
                if (weaponButtons[i].priceText) weaponButtons[i].priceText.color = equipped ? disabledColor : weaponButtons[i].GetDefaultPriceTextColor();
            }
        }
    }

    public void OnClickButton(WeaponBASE weaponPrefab)
    {
        //be sure there is a weapon and player has weapon component
        if (weaponPrefab != null && mainInteracting && mainInteracting.GetSavedComponent<WeaponComponent>())
        {
            //if current weapons are full, destroy current equipped (to not drop it)
            if (mainInteracting.GetSavedComponent<WeaponComponent>().IsFull())
                Destroy(mainInteracting.GetSavedComponent<WeaponComponent>().CurrentWeapon.gameObject);

            //pick new weapon
            mainInteracting.GetSavedComponent<WeaponComponent>().PickWeaponPrefab(weaponPrefab);

            //save stats for next scene
            if (GameManager.instance && GameManager.instance.levelManager)
                GameManager.instance.SaveStats(GameManager.instance.levelManager.Players.ToArray());

            //update UI
            UpdateUI();
        }
    }
}
