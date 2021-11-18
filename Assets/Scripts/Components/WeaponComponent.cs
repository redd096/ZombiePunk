using UnityEngine;
using Sirenix.OdinInspector;

public class WeaponComponent : MonoBehaviour
{

    [Header("Instantiate default weapon")]
    [ValueDropdown("@OdinUtility.GetListOfAssets<WeaponBASE>()", AppendNextDrawer = true)]
    [SerializeField] WeaponBASE weaponPrefab = default;

    [Header("DEBUG")]
    [ReadOnly] public WeaponBASE CurrentWeapon = default;

    //events
    public System.Action onPickEquip { get; set; }
    public System.Action onDropEquip { get; set; }

    Redd096Main owner;

    void Awake()
    {
        //get references
        owner = GetComponent<Redd096Main>();

        //instantiate and equip default weapon
        if (weaponPrefab)
            PickWeapon(Instantiate(weaponPrefab));
    }

    /// <summary>
    /// Set current Weapon
    /// </summary>
    /// <param name="weapon"></param>
    /// <param name="owner"></param>
    public void PickWeapon(WeaponBASE weapon)
    {
        //be sure to not have other weapons
        if (CurrentWeapon != null)
            DropWeapon();

        //set current weapon
        CurrentWeapon = weapon;

        //set equip owner
        CurrentWeapon.PickWeapon(owner);

        //call event
        onPickEquip?.Invoke();
    }

    /// <summary>
    /// Drop current Weapon
    /// </summary>
    public void DropWeapon()
    {
        if (CurrentWeapon == null)
            return;

        //remove weapon's owner
        CurrentWeapon.DropWeapon();

        //remove current weapon
        CurrentWeapon = null;

        //call event
        onDropEquip?.Invoke();
    }
}
