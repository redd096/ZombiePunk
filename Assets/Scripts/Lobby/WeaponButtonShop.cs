using UnityEngine;
using UnityEngine.UI;

public class WeaponButtonShop : MonoBehaviour
{
    public Button button;
    public Image imageWeapon;
    public Text nameText;
    public Text priceText;

    [Header("Used in the shop, to show a text on the button when player hasn't reached the level to unlock this")]
    public GameObject objectToActivateWhenButtonIsLocked;

    [Header("Used in the inventory, to show which weapon's slot player has selected")]
    public GameObject objectToActivateWhenSelectSlot;

    bool initialized = false;
    Color defaultNameTextColor;
    Color defaultPriceTextColor;

    /// <summary>
    /// Save default text colors (will be saved only first time)
    /// </summary>
    public void Init()
    {
        //do once
        if (initialized)
            return;

        initialized = true;

        //save text colors
        if (nameText)
            defaultNameTextColor = nameText.color;
        if (priceText)
            defaultPriceTextColor = priceText.color;
    }

    /// <summary>
    /// Return default price text color. For example to re-enable the button
    /// </summary>
    /// <returns></returns>
    public Color GetDefaultNameTextColor()
    {
        return defaultNameTextColor;
    }

    /// <summary>
    /// Return default price text color. For example to re-enable the button
    /// </summary>
    /// <returns></returns>
    public Color GetDefaultPriceTextColor()
    {
        return defaultPriceTextColor;
    }
}
