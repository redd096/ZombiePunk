using UnityEngine;
using redd096.Attributes;
using redd096.GameTopDown2D;

public class ComboComponent : MonoBehaviour
{
    public enum EComboState { Charging, Locked, Active }

    [Header("Combo Rules")]
    public int ComboToReach = 100;
    public float DecreaseEverySecond = 1;
    public bool WhenReachLimitIsLockedForever = true;
    [DisableIf("WhenReachLimitIsLockedForever")] public float LockedForThisTime = 10;

    [Header("On Active")]
    public WeaponBASE WeaponToSet = default;
    public float DurationWeapon = 10;

    [Header("Read Only")]
    [ReadOnly] [SerializeField] float currentCombo = 0;
    [ReadOnly] [SerializeField] float timer = 0;
    [ReadOnly] [SerializeField] EComboState comboState = EComboState.Charging;

    public float CurrentCombo => currentCombo;
    public float TimerLocked => comboState == EComboState.Locked ? timer : 0;
    public float TimerWeapon => comboState == EComboState.Active ? timer : 0;
    public EComboState ComboState => comboState;
    public WeaponBASE[] SavedWeapons => savedWeapons;
    public int SavedIndexWeapon => savedIndexWeapon;

    //events
    public System.Action onAddPoint { get; set; }
    public System.Action onReachLimit { get; set; }
    public System.Action onUnlockLimit { get; set; }
    public System.Action onActive { get; set; }
    public System.Action onDeactive { get; set; }

    //save weapons before set the special one
    WeaponComponent weaponComponent;
    WeaponBASE[] savedWeapons = default;
    int savedIndexWeapon = 0;

    void Awake()
    {
        //get references
        weaponComponent = GetComponent<WeaponComponent>();        
    }

    void Update()
    {
        //decrease combo, while player keep charging the combo
        if (comboState == EComboState.Charging)
        {
            OnChargingState();
        }
        //when reach limit, keep locked for few times or until player press to active
        else if (comboState == EComboState.Locked)
        {
            OnLockedState();
        }
        //when active, use weapon for few seconds, then back to charging state
        else
        {
            OnActiveState();
        }
    }

    #region private API

    void OnChargingState()
    {
        //decrease combo, while player keep charging the combo
        currentCombo = Mathf.Max(0, currentCombo - DecreaseEverySecond * Time.deltaTime);
    }

    void ReachedLimit()
    {
        //when reach limit, set locked state
        comboState = EComboState.Locked;
        timer = LockedForThisTime;

        //call events
        onReachLimit?.Invoke();
    }

    void OnLockedState()
    {
        //if locked forever, do nothing until player press button to activate it
        if (WhenReachLimitIsLockedForever)
            return;

        //else wait few times, then back to charging state to start decrease again
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            comboState = EComboState.Charging;

            //call events
            onUnlockLimit?.Invoke();
        }
    }

    void StartActiveState()
    {
        //when press button, set active state
        comboState = EComboState.Active;
        timer = DurationWeapon;
        currentCombo = 0;

        //set super weapon
        if (weaponComponent)
        {
            //save weapons
            savedWeapons = weaponComponent.CurrentWeapons.Clone() as WeaponBASE[];
            savedIndexWeapon = weaponComponent.IndexEquippedWeapon;

            //drop every weapon
            for (int i = 0; i < savedWeapons.Length; i++)
                weaponComponent.DropWeapon();

            //and deactive
            for (int i = 0; i < savedWeapons.Length; i++)
                if (savedWeapons[i])
                    savedWeapons[i].gameObject.SetActive(false);

            //set only one weapon and equip special one
            weaponComponent.SetMaxWeapons(1);
            weaponComponent.PickWeaponPrefab(WeaponToSet);
        }

        //call events
        onActive?.Invoke();
    }

    void OnActiveState()
    {
        //wait few times
        timer -= Time.deltaTime;

        //then stop combo and back to charging state
        if (timer < 0)
        {
            comboState = EComboState.Charging;
            StopActiveState();
        }
    }

    void StopActiveState()
    {
        //remove super weapon
        if (weaponComponent)
        {
            //destroy super weapon and reset number of weapons
            Destroy(weaponComponent.CurrentWeapon.gameObject);
            weaponComponent.SetMaxWeapons(savedWeapons.Length);

            //re-pick every weapon
            for (int i = 0; i < savedWeapons.Length; i++)
                if (savedWeapons[i])
                    weaponComponent.PickWeapon(savedWeapons[i], i);

            //and switch to previous equipped weapon
            weaponComponent.SwitchWeaponTo(savedIndexWeapon);

            //reset vars
            savedWeapons = null;
            savedIndexWeapon = 0;
        }

        //call events
        onDeactive?.Invoke();
    }

    #endregion

    #region public API

    /// <summary>
    /// Add combo and check if reach limit
    /// </summary>
    /// <param name="comboToAdd"></param>
    public void AddCombo(int comboToAdd)
    {
        //if charging state
        if (comboState == EComboState.Charging)
        {
            //add combo
            currentCombo += comboToAdd;

            //call event
            onAddPoint?.Invoke();

            //if reach limit
            if (currentCombo >= ComboToReach)
            {
                currentCombo = ComboToReach;    //clamp
                ReachedLimit();
            }
        }
    }

    /// <summary>
    /// When combo is at limit, press a button to activate it
    /// </summary>
    public void ActiveCombo()
    {
        //if locked state
        if (comboState == EComboState.Locked)
        {
            //active this combo
            StartActiveState();
        }
    }

    #endregion
}
