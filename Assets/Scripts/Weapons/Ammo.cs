using UnityEngine;
using redd096;

public class Ammo : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] string typeOfAmmo = "GunAmmo";
    [SerializeField] int quantity = 1;

    [Header("Destroy when instantiated")]
    [Tooltip("0 = never destroy")] [SerializeField] [Min(0)] float timeBeforeDestroy = 0;

    public void Interact(InteractComponent whoInteract)
    {
        WeaponComponent weaponComponent = whoInteract.GetComponent<WeaponComponent>();
        if (weaponComponent)
        {

        }
    }
}
