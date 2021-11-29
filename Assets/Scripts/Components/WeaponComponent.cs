using UnityEngine;
using NaughtyAttributes;

namespace redd096
{
    [AddComponentMenu("redd096/Components/Weapon Component")]
    public class WeaponComponent : MonoBehaviour
    {
        [Header("Instantiate default weapon")]
        [SerializeField] WeaponBASE weaponPrefab = default;

        [Header("Destroy Weapon On Death (necessary HealthComponent - default get from this gameObject)")]
        [SerializeField] bool dropWeaponOnDeath = true;
        [SerializeField] bool destroyWeaponOnDeath = true;
        [EnableIf("destroyWeaponOnDeath")] [SerializeField] HealthComponent healthComponent;

        [Header("DEBUG")]
        [ReadOnly] public WeaponBASE CurrentWeapon = default;

        //events
        public System.Action onPickEquip { get; set; }
        public System.Action onDropEquip { get; set; }

        Character owner;

        void Awake()
        {
            //get references
            owner = GetComponent<Character>();

            //instantiate and equip default weapon
            if (weaponPrefab)
                PickWeapon(Instantiate(weaponPrefab));
        }

        void OnEnable()
        {
            //get references
            if (healthComponent == null) 
                healthComponent = GetComponent<HealthComponent>();

            //add events
            if(healthComponent)
            {
                healthComponent.onDie += OnDie;
            }
        }

        void OnDisable()
        {
            //remove events
            if (healthComponent)
            {
                healthComponent.onDie -= OnDie;
            }
        }

        void OnDie()
        {
            //save weapon to destroy
            GameObject weaponToDestroy = CurrentWeapon ? CurrentWeapon.gameObject : null;

            //drop weapon on death, if setted
            if (dropWeaponOnDeath && CurrentWeapon)
                DropWeapon();

            //destroy weapon on death, if setted
            if (destroyWeaponOnDeath && weaponToDestroy)
                Destroy(weaponToDestroy);
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
}