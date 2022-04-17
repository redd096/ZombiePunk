using UnityEngine;
using UnityEngine.UI;

public class WeaponButtonShop : MonoBehaviour
{
    public Button button;
    public Image imageWeapon;
    public Text nameText;
    public Text priceText;
    [Tooltip("Used in the shop, to show a text on the button when player hasn't reached the level to unlock this")] public GameObject objectToActivateWhenButtonIsLocked;

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

        //get reference
        if (button == null) button = GetComponent<Button>();
        if (imageWeapon == null) imageWeapon = GetComponent<Image>();

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
