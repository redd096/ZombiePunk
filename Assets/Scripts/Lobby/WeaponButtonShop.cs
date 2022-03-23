using UnityEngine;
using UnityEngine.UI;

public class WeaponButtonShop : MonoBehaviour
{
    public Button button;
    public Text nameText;
    public Text priceText;

    Color defaultNameTextColor;
    Color defaultPriceTextColor;

    void Awake()
    {
        //save text colors (will be deacivated if not interactable)
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
