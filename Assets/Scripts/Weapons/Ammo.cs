using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Weapons/Ammo")]
    public class Ammo : PickUpBASE
    {
        [Header("Ammo")]
        [SerializeField] string ammoType = "GunAmmo";
        [SerializeField] int quantity = 1;
        [Tooltip("Can pick when full of this type of ammo? If true, this object will be destroyed, but no ammo will be added")] [SerializeField] bool canPickAlsoIfFull = false;

        public string AmmoType => ammoType;

        public override void PickUp()
        {
            //check if hit has component
            AdvancedWeaponComponent whoHitComponent = whoHit.GetSavedComponent<AdvancedWeaponComponent>();
            if (whoHitComponent)
            {
                //if can pick when full, or is not full
                if (canPickAlsoIfFull || whoHitComponent.IsFullOfAmmo(ammoType) == false)
                {
                    //pick and add quantity
                    whoHitComponent.AddAmmo(ammoType, quantity);
                    OnPick();
                }
                //else fail pick
                else
                {
                    OnFailPick();
                }
            }
        }
    }
}