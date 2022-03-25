using System.Collections.Generic;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

public class SavesBetweenScenes
{
    public float CurrentHealth;
    public WeaponBASE[] WeaponsPrefabs;
    public Dictionary<string, int> CurrentAmmos;
}

[System.Serializable]
public class SaveClassMoney
{
    public int Money;
}

[System.Serializable]
public class SaveClassBoughtWeapons
{
    public List<WeaponBASE> BoughtWeapons;
}

[AddComponentMenu("redd096/Singletons/Game Manager")]
[DefaultExecutionOrder(-100)]
public class GameManager : Singleton<GameManager>
{
    [Header("Lock 60 FPS")]
    [SerializeField] bool lock60Fps = true;

    //se si vuole fare multiplayer, si salva un array per ogni ID
    //nel menu customizzazione si aggiunge uno script ai prefab per passare il PointerEventData al click, per sapere l'ID di chi ha cliccato
    //OLD
    CustomizeData[] currentCustomizations = default;
    WeaponBASE[] currentWeaponsPrefabs = default;

    public UIManager uiManager { get; private set; }
    public LevelManager levelManager { get; private set; }

    //saves in game
    SavesBetweenScenes savedStats;          //to move between scenes, so currently equipped

    //save and load in json
    public const string MONEY_SAVENAME = "Money";
    public const string BOUGHTWEAPONS_SAVENAME = "BoughtWeapons";
    SaveClassMoney savedMoney;
    SaveClassBoughtWeapons savedBoughtWeapons;

    protected override void Awake()
    {
        base.Awake();

        //when start game, load files
        if (instance)
        {
            savedMoney = SaveLoadJSON.Load<SaveClassMoney>(MONEY_SAVENAME);
            savedBoughtWeapons = SaveLoadJSON.Load<SaveClassBoughtWeapons>(BOUGHTWEAPONS_SAVENAME);
        }
    }

    protected override void SetDefaults()
    {
        //get references
        uiManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();

        //lock 60 fps or free
        Application.targetFrameRate = lock60Fps ? 60 : -1;

        //load stats to players
        if (levelManager)
        {
            LoadStats();
        }
        //reset when move to a level without LevelManager
        else
        {
            savedStats = null;
        }
    }

    void OnValidate()
    {
        //lock 60 fps or free
        Application.targetFrameRate = lock60Fps ? 60 : -1;
    }

    #region move players between scenes

    public void SaveStats(Character[] players)
    {
        //save stats for players
        foreach (Character player in players)
        {
            //health and ammos
            savedStats = new SavesBetweenScenes();
            if (player.GetSavedComponent<HealthComponent>()) savedStats.CurrentHealth = player.GetSavedComponent<HealthComponent>().CurrentHealth;
            if (player.GetSavedComponent<AdvancedWeaponComponent>()) savedStats.CurrentAmmos = new Dictionary<string, int>(player.GetSavedComponent<AdvancedWeaponComponent>().CurrentAmmos_NotSafe);

            //foreach weapon save prefab
            if (player.GetSavedComponent<WeaponComponent>())
            {
                savedStats.WeaponsPrefabs = new WeaponBASE[player.GetSavedComponent<WeaponComponent>().CurrentWeapons.Length];
                for (int i = 0; i < savedStats.WeaponsPrefabs.Length; i++)
                {
                    if (player.GetSavedComponent<WeaponComponent>().CurrentWeapons[i])
                    {
                        savedStats.WeaponsPrefabs[i] = player.GetSavedComponent<WeaponComponent>().CurrentWeapons[i].WeaponPrefab;
                    }
                }
            }
        }
    }

    void LoadStats()
    {
        if (savedStats == null)
            return;

        foreach (Character player in FindObjectsOfType<Character>())
        {
            //foreach player, load stats
            if (player.CharacterType == Character.ECharacterType.Player)
            {
                //health and ammos
                if (player.GetSavedComponent<HealthComponent>()) player.GetSavedComponent<HealthComponent>().CurrentHealth = savedStats.CurrentHealth;
                if (player.GetSavedComponent<AdvancedWeaponComponent>()) player.GetSavedComponent<AdvancedWeaponComponent>().CurrentAmmos_NotSafe = new Dictionary<string, int>(savedStats.CurrentAmmos);

                //weapons will be loaded automatically from WeaponComponent
            }
        }
    }

    public SavesBetweenScenes GetSavedStats()
    {
        return savedStats;
    }

    public bool HasSavedStats()
    {
        return savedStats != null;
    }

    #endregion

    #region save and load json

    public void Save<T>(T classToSave)
    {
        //save money class
        if (typeof(T) == typeof(SaveClassMoney))
        {
            SaveLoadJSON.Save(MONEY_SAVENAME, classToSave);
            savedMoney = classToSave as SaveClassMoney;
        }
        //or bought weapons class
        else
        {
            SaveLoadJSON.Save(BOUGHTWEAPONS_SAVENAME, classToSave);
            savedBoughtWeapons = classToSave as SaveClassBoughtWeapons;
        }
    }

    public T Load<T>() where T : class
    {
        //return money class
        if (typeof(T) == typeof(SaveClassMoney))
        {
            //if not loaded at start, then return an empty class
            if (savedMoney == null) 
                savedMoney = new SaveClassMoney();

            return savedMoney as T;
        }
        //or bought weapons class
        else
        {
            //if not loaded at start, then return an empty class
            if (savedBoughtWeapons == null)
                savedBoughtWeapons = new SaveClassBoughtWeapons();

            return savedBoughtWeapons as T;
        }
    }

    public void ClearSave<T>()
    {
        //delete money class
        if (typeof(T) == typeof(SaveClassMoney))
        {
            SaveLoadJSON.DeleteData(MONEY_SAVENAME);
            savedMoney = new SaveClassMoney();
        }
        //or bought weapons class
        else
        {
            SaveLoadJSON.DeleteData(BOUGHTWEAPONS_SAVENAME);
            savedBoughtWeapons = new SaveClassBoughtWeapons();
        }
    }

    #endregion

    #region OLD customizations API

    /// <summary>
    /// Set customizations for next scene
    /// </summary>
    /// <param name="customizations"></param>
    public void SetCustomizations(CustomizeData[] customizations)
    {
        currentCustomizations = customizations;
    }

    /// <summary>
    /// Return current list of customizations
    /// </summary>
    /// <returns></returns>
    public CustomizeData[] GetCustomizations()
    {
        return currentCustomizations;
    }

    /// <summary>
    /// Reset customizations
    /// </summary>
    public void ClearCustomizations()
    {
        currentCustomizations = new CustomizeData[0];
    }

    /// <summary>
    /// Add customizations to weapon
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public WeaponBASE AddCustomizationsToWeapon(WeaponBASE weapon)
    {
        //if there are customizations and is a range weapon
        if (currentCustomizations != null && currentCustomizations.Length > 0 && weapon is WeaponRange weaponRange)
        {
            //add every customization
            for (int i = 0; i < currentCustomizations.Length; i++)
            {
                if (currentCustomizations[i])
                    weaponRange = currentCustomizations[i].AddRangeCustomizations(weaponRange);
            }

            return weaponRange;
        }

        //else return normal weapon
        return weapon;
    }

    #endregion

    #region OLD weapons API

    /// <summary>
    /// Set weapons for next scene
    /// </summary>
    /// <param name="weaponsPrefabs"></param>
    public void SetWeapons(WeaponBASE[] weaponsPrefabs)
    {
        currentWeaponsPrefabs = weaponsPrefabs;
    }

    /// <summary>
    /// Return current weapons
    /// </summary>
    /// <returns></returns>
    public WeaponBASE[] GetWeapons()
    {
        return currentWeaponsPrefabs;
    }

    /// <summary>
    /// Return if there are weapons saved
    /// </summary>
    /// <returns></returns>
    public bool HasWeaponsSaved()
    {
        return currentWeaponsPrefabs != null && currentWeaponsPrefabs.Length > 0;
    }

    /// <summary>
    /// Reset weapons
    /// </summary>
    public void ClearWeapons()
    {
        currentWeaponsPrefabs = null;
    }

    #endregion
}