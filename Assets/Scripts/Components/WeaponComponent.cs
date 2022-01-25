using UnityEngine;
using NaughtyAttributes;

namespace redd096
{
    [AddComponentMenu("redd096/Components/Weapon Component")]
    public class WeaponComponent : MonoBehaviour
    {
        public enum EWeaponOnDeath { None, OnlyEquippedOne, EveryWeapon }

        [Header("Instantiate default weapons")]
        [SerializeField] WeaponBASE[] weaponsPrefabs = default;

        [Header("Number of Weapons")]
        [Min(1)] [SerializeField] int maxWeapons = 2;

        [Header("Destroy Weapon On Death (necessary HealthComponent - default get from this gameObject)")]
        [SerializeField] EWeaponOnDeath dropWeaponOnDeath = EWeaponOnDeath.None;
        [SerializeField] EWeaponOnDeath destroyWeaponOnDeath = EWeaponOnDeath.EveryWeapon;
        [SerializeField] HealthComponent healthComponent = default;

        [Header("DEBUG")]
        [ReadOnly] public WeaponBASE[] CurrentWeapons = default;    //it will be always the same size of Max Weapons
        [ShowNonSerializedField] int indexEquippedWeapon = 0;       //it will be always the correct index, or zero

        public WeaponBASE EquippedWeapon => CurrentWeapons != null && indexEquippedWeapon < CurrentWeapons.Length ? CurrentWeapons[indexEquippedWeapon] : null;

        //events
        public System.Action onPickWeapon { get; set; }         //called at every pick
        public System.Action onDropWeapon { get; set; }         //called at every drop
        public System.Action onChangeWeapon { get; set; }       //called at every pick and every drop

        Character owner;
        Transform currentWeaponsParent;

        void Awake()
        {
            //set vars
            CurrentWeapons = new WeaponBASE[maxWeapons];
            currentWeaponsParent = new GameObject(name + "'s Weapons").transform;

            //get references
            owner = GetComponent<Character>();

            //instantiate default weapons
            SetDefaultWeapons();

            //set index equipped weapon
            UpdateIndexEquippedWeapon();
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

        #region private API

        void SetDefaultWeapons()
        {
            //if player, try get weapon saved in game manager
            if (owner && owner.CharacterType == Character.ECharacterType.Player && GameManager.instance && GameManager.instance.HasWeaponsSaved())
            {
                WeaponBASE[] weapons = GameManager.instance.GetWeapons();
                for (int i = 0; i < CurrentWeapons.Length; i++)
                {
                    if (i < weapons.Length)
                    {
                        if (weapons[i])
                            PickWeapon(Instantiate(weapons[i]));
                    }
                    else
                        break;
                }
            }
            //else (if not player or there is no weapon in game manager), instantiate and equip default weapons
            else if (weaponsPrefabs != null && weaponsPrefabs.Length > 0)
            {
                for (int i = 0; i < CurrentWeapons.Length; i++)
                {
                    if (i < weaponsPrefabs.Length)
                    {
                        if (weaponsPrefabs[i])
                            PickWeapon(Instantiate(weaponsPrefabs[i]));
                    }
                    else
                        break;
                }
            }

            ////if has a weapon and is a Player, add customizations saved in GameManager
            //if (EquippedWeapon && owner.CharacterType == Character.ECharacterType.Player && GameManager.instance)
            //{
            //    GameManager.instance.AddCustomizationsToWeapon(EquippedWeapon);
            //}
        }

        void OnDie(HealthComponent whoDied)
        {
            //destroy equipped weapon on death
            if (destroyWeaponOnDeath == EWeaponOnDeath.OnlyEquippedOne)
            {
                if (CurrentWeapons != null && indexEquippedWeapon < CurrentWeapons.Length && CurrentWeapons[indexEquippedWeapon])
                    Destroy(CurrentWeapons[indexEquippedWeapon]);
            }
            //or destroy every weapon
            else if (destroyWeaponOnDeath == EWeaponOnDeath.EveryWeapon)
            {
                if (CurrentWeapons != null)
                    for (int i = 0; i < CurrentWeapons.Length; i++)
                        if (CurrentWeapons[i])
                            Destroy(CurrentWeapons[i]);
            }

            //drop equipped weapon on death
            if (dropWeaponOnDeath == EWeaponOnDeath.OnlyEquippedOne)
            {
                DropWeapon(indexEquippedWeapon);
            }
            //or drop every weapon
            else if (dropWeaponOnDeath == EWeaponOnDeath.EveryWeapon)
            {
                if (CurrentWeapons != null)
                    for (int i = 0; i < CurrentWeapons.Length; i++)
                        DropWeapon(i);
            }
        }

        void UpdateIndexEquippedWeapon()
        {
            //start from current weapon or last index array (if lower) - if current weapons length is 0, index will be set to 0
            for (int i = Mathf.Min(indexEquippedWeapon, Mathf.Max(0, CurrentWeapons.Length - 1)); i >= 0; i--)
            {
                //set first weapon not null
                indexEquippedWeapon = i;
                if (indexEquippedWeapon < CurrentWeapons.Length && CurrentWeapons[indexEquippedWeapon] != null)
                    break;
            }
        }

        #endregion

        #region public API

        /// <summary>
        /// Pick Weapon (add in an empty slot or replace equipped one with it)
        /// </summary>
        /// <param name="weapon"></param>
        public void PickWeapon(WeaponBASE weapon)
        {
            if (CurrentWeapons == null || CurrentWeapons.Length <= 0)
                return;

            //find empty slot (or equipped one)
            int index = indexEquippedWeapon;
            for (int i = 0; i < CurrentWeapons.Length; i++)
            {
                if (CurrentWeapons[i] == null)
                {
                    index = i;
                    break;
                }
            }

            //if there is already a weapon equipped, drop it
            if (CurrentWeapons[index] != null)
                DropWeapon(index);

            //pick weapon
            CurrentWeapons[index] = weapon;

            //set owner and parent
            if (weapon)
            {
                weapon.PickWeapon(owner);
                weapon.transform.SetParent(currentWeaponsParent);

                //if not equipped, deactive
                if (index != indexEquippedWeapon) weapon.gameObject.SetActive(false);
            }

            //set index equipped weapon
            UpdateIndexEquippedWeapon();

            //call events
            onPickWeapon?.Invoke();
            onChangeWeapon?.Invoke();
        }

        /// <summary>
        /// Drop Weapon at index
        /// </summary>
        public void DropWeapon(int index)
        {
            if (CurrentWeapons == null || index >= CurrentWeapons.Length)
                return;

            //remove owner and parent
            if (CurrentWeapons[index])
            {
                CurrentWeapons[index].DropWeapon();
                CurrentWeapons[index].transform.SetParent(null);

                //if not equipped, reactive
                if (index != indexEquippedWeapon) CurrentWeapons[index].gameObject.SetActive(true);
            }

            //drop weapon
            CurrentWeapons[index] = null;

            //set index equipped weapon
            UpdateIndexEquippedWeapon();

            //call event
            onDropWeapon?.Invoke();
            onChangeWeapon?.Invoke();
        }

        /// <summary>
        /// Drop equipped weapon
        /// </summary>
        public void DropWeapon()
        {
            DropWeapon(indexEquippedWeapon);
        }

        /// <summary>
        /// Set max number of weapons, and update array
        /// </summary>
        /// <param name="maxWeapons">Min number is 1</param>
        public void SetMaxWeapons(int maxWeapons)
        {
            //set max weapons
            this.maxWeapons = maxWeapons;

            //copy weapons
            WeaponBASE[] weapons = new WeaponBASE[maxWeapons];
            if (CurrentWeapons != null)
            {
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (i < CurrentWeapons.Length)
                        weapons[i] = CurrentWeapons[i];
                    else
                        break;
                }
            }
            CurrentWeapons = weapons;

            //set index equipped weapon
            UpdateIndexEquippedWeapon();
        }

        #endregion
    }
}