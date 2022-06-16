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

        AdvancedWeaponComponent whoHitComponent;

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);

            //save component from magnet or hit
            if (whoHitComponent == null)
            {
                if (player && whoHit == null)
                    whoHitComponent = player.GetComponent<AdvancedWeaponComponent>();
                else if (whoHit)
                    whoHitComponent = whoHit.GetSavedComponent<AdvancedWeaponComponent>();
            }
        }

        public override void PickUp()
        {
            //if can pick
            if (CanPickUp())
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

        protected override bool CanPickUp()
        {
            //there is weapon component, and is not full of ammo or can pick anyway
            return whoHitComponent && (whoHitComponent.IsFullOfAmmo(ammoType) == false || canPickAlsoIfFull);
        }
    }
}