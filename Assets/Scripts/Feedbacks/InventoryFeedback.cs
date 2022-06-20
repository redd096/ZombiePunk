using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;

[System.Serializable]
public struct InventoryFeedbackStruct
{
    public GameObject objectToActivate;
    public WeaponBASE necessaryWeapon;
}

public class InventoryFeedback : MonoBehaviour
{
    [Header("Inventory Feedback")]
    [SerializeField] bool showAlsoEquippedWeapons = true;
    [SerializeField] InventoryFeedbackStruct[] objectsToActivate = default;

    InventoryInteract inventory;

    private void OnEnable()
    {
        //get references
        inventory = GetComponentInParent<InventoryInteract>();
        
        //add events
        if (inventory)
        {
            inventory.onUpdateInventory += OnUpdateInventory;
        }
    }

    private void OnDisable()
    {
        //remove events
        if (inventory)
        {
            inventory.onUpdateInventory -= OnUpdateInventory;
        }
    }

    private void OnUpdateInventory(List<ISellable> alreadyBoughtWeapons, Redd096Main mainInteracting)
    {
        //activate if already bought, else not
        foreach (InventoryFeedbackStruct obj in objectsToActivate)
        {
            if (obj.objectToActivate)
            {
                obj.objectToActivate.SetActive(CanActivate(alreadyBoughtWeapons, obj.necessaryWeapon, mainInteracting));
            }
        }
    }

    bool CanActivate(List<ISellable> alreadyBoughtWeapons, ISellable sellable, Redd096Main mainInteracting)
    {
        if (mainInteracting == null || sellable == null)
            return false;

        //if already bought
        if (alreadyBoughtWeapons.Contains(sellable))
        {
            //show always
            if (showAlsoEquippedWeapons)
            {
                return true;
            }
            //or only when not equipped
            else
            {
                return IsEquipped(sellable, mainInteracting) == false;
            }
        }

        return false;
    }

    bool IsEquipped(ISellable sellable, Redd096Main mainInteracting)
    {
        //check if already equipped weapon
        if (sellable is WeaponBASE)
        {
            WeaponBASE sellableWeapon = sellable as WeaponBASE;
            if (mainInteracting && mainInteracting.GetSavedComponent<WeaponComponent>())
            {
                //if inside list of current weapons, set is equipped
                foreach (WeaponBASE weapon in mainInteracting.GetSavedComponent<WeaponComponent>().CurrentWeapons)
                {
                    //check if is same prefab
                    if (weapon && weapon.WeaponPrefab == sellableWeapon)
                    {
                        return true;
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
                    return true;
            }
        }

        return false;
    }
}
