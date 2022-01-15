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
        [EnableIf(EConditionOperator.Or, "dropWeaponOnDeath", "destroyWeaponOnDeath")] [SerializeField] HealthComponent healthComponent = default;

        [Header("DEBUG")]
        [ReadOnly] public WeaponBASE CurrentWeapon = default;

        //events
        public System.Action onPickWeapon { get; set; }         //called at every pick
        public System.Action onDropWeapon { get; set; }         //called at every drop
        public System.Action onChangeWeapon { get; set; }       //called at every pick and every drop

        Character owner;

        void Awake()
        {
            //get references
            owner = GetComponent<Character>();

            //if player, try get weapon saved in game manager
            if(owner && owner.CharacterType == Character.ECharacterType.Player && GameManager.instance && GameManager.instance.GetWeapon() != null)
            {
                PickWeapon(Instantiate(GameManager.instance.GetWeapon()));
            }
            //else (if not player or there is no weapon in game manager), instantiate and equip default weapon
            else if (weaponPrefab)
            {
                PickWeapon(Instantiate(weaponPrefab));
            }

            //if has a weapon and is a Player, add customizations saved in GameManager
            if (CurrentWeapon && owner.CharacterType == Character.ECharacterType.Player && GameManager.instance)
            {
                GameManager.instance.AddCustomizationsToWeapon(CurrentWeapon);
            }
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

        void OnDie(HealthComponent whoDied)
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

            //call events
            onPickWeapon?.Invoke();
            onChangeWeapon?.Invoke();
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
            onDropWeapon?.Invoke();
            onChangeWeapon?.Invoke();
        }
    }
}