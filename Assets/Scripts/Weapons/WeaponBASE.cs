using UnityEngine;
using NaughtyAttributes;

namespace redd096
{
    public abstract class WeaponBASE : MonoBehaviour
    {
        [Header("Weapon BASE")]
        public string WeaponName = "Weapon Name";
        public int WeaponPrice = 10;
        [SerializeField] bool destroyWeaponOnDrop = true;

        [Header("DEBUG")]
        [ReadOnly] public Character Owner;

        //events
        public System.Action onPickWeapon { get; set; }
        public System.Action onDropWeapon { get; set; }

        #region public API

        /// <summary>
        /// Set owner to look at
        /// </summary>
        /// <param name="owner"></param>
        public void PickWeapon(Character owner)
        {
            Owner = owner;

            //call event
            onPickWeapon?.Invoke();
        }

        /// <summary>
        /// Remove owner
        /// </summary>
        public void DropWeapon()
        {
            Owner = null;

            //be sure to reset attack vars
            ReleaseAttack();

            //call event
            onDropWeapon?.Invoke();

            //destroy weapon, if setted
            if(destroyWeaponOnDrop)
                Destroy(gameObject);
        }

        #endregion

        #region abstracts

        public abstract void PressAttack();
        public abstract void ReleaseAttack();

        #endregion
    }
}