using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;
using redd096.Attributes;
using System.Linq;

//when add new classes to save in json, just create a new class (at the end of this file) and add in the array classesToSave
//when add new variable to load and save between scenes, just add to SavesBetweenScenes class and in functions SaveStats and LoadStats

[DefaultExecutionOrder(-99)]
public class SavesManager : Singleton<SavesManager>
{
    enum EClearSaveCondition { ClearWhenEnterInLobby, ClearWhenEnterInMainMenu, ClearWhenEnterInBoth, Never }

    [Header("Save between scenes")]
    [SerializeField] bool saveHealth = true;
    [SerializeField] bool saveAmmo = true;
    [SerializeField] bool savePerks = true;
    [SerializeField] bool saveWeapons = true;
    [SerializeField] bool saveIndexEquippedWeapon = true;
    [Space]
    [SerializeField] EClearSaveCondition clearStatsOnExit = EClearSaveCondition.ClearWhenEnterInMainMenu;

    [Header("Save Checkpoint (only greater than zero)")]
    [ReadOnly] [SerializeField] int currentCheckpoint = 0;
    [SerializeField] EClearSaveCondition clearCheckpointOnExit = EClearSaveCondition.ClearWhenEnterInBoth;
    [Space]
    [SerializeField] int overwriteCheckpoint = 10;
    [Button] void OverwriteCheckpoint() => SaveCheckpoint(overwriteCheckpoint);
    [Button] void ForceLoadCheckpoint() => LoadCheckpoint();

    //save and load in json
    ISaveClass[] classesToSave = new ISaveClass[] { new SaveClassMoney(), new SaveClassBoughtElements(), new SaveClassLevelReached() };

    //save and load between scenes
    SavesBetweenScenes savedStats;

    //save and load checkpoint
    SavesCheckpoint reachedCheckpoint;

    protected override void Awake()
    {
        base.Awake();

        //when start game, load json files
        if (instance)
        {
            for (int i = 0; i < classesToSave.Length; i++)
            {
                //set only if file is not null (otherwise, keep empty class as setted in the array above)
                ISaveClass obj = SaveLoadJSON.Load(classesToSave[i].key, classesToSave[i].type) as ISaveClass;
                if (obj != null) classesToSave[i] = obj;
            }
        }
    }

    protected override void SetDefaults()
    {
        base.SetDefaults();

        //when move to a level without level manager (MAIN MENU)
        if (GameManager.instance == false || GameManager.instance.levelManager == false)
        {
            //clear saves between scenes
            if (clearStatsOnExit == EClearSaveCondition.ClearWhenEnterInMainMenu || clearStatsOnExit == EClearSaveCondition.ClearWhenEnterInBoth)
                ClearStats();

            //clear checkpoint
            if (clearCheckpointOnExit == EClearSaveCondition.ClearWhenEnterInMainMenu || clearCheckpointOnExit == EClearSaveCondition.ClearWhenEnterInBoth)
                ClearCheckpoint();
        }
        //when move to lobby scene
        else if (FindObjectOfType<MapInteract>() != null)
        {
            //clear saves between scenes
            if (clearStatsOnExit == EClearSaveCondition.ClearWhenEnterInLobby || clearStatsOnExit == EClearSaveCondition.ClearWhenEnterInBoth)
                ClearStats();

            //clear checkpoint
            if (clearCheckpointOnExit == EClearSaveCondition.ClearWhenEnterInLobby || clearCheckpointOnExit == EClearSaveCondition.ClearWhenEnterInBoth)
                ClearCheckpoint();
        }
    }

    #region save and load json

    public void Save<T>(T classToSave)
    {
        for (int i = 0; i < classesToSave.Length; i++)
        {
            //find type in classes to save
            if (classesToSave[i].type == typeof(T))
            {
                //save in file and update variable
                SaveLoadJSON.Save(classesToSave[i].key, classToSave);
                classesToSave[i] = classToSave as ISaveClass;
                break;
            }
        }
    }

    public T Load<T>() where T : class
    {
        for (int i = 0; i < classesToSave.Length; i++)
        {
            //find type in classes to save
            if (classesToSave[i].type == typeof(T))
            {
                //return it (can't be null)
                return classesToSave[i] as T;
            }
        }

        return null;
    }

    public void ClearSave<T>()
    {
        for (int i = 0; i < classesToSave.Length; i++)
        {
            //find type in classes to save
            if (classesToSave[i].type == typeof(T))
            {
                //reset to an empty class (but never be null)
                SaveLoadJSON.DeleteData(classesToSave[i].key);
                classesToSave[i] = classesToSave[i].GetEmptyClass();
                break;
            }
        }
    }

    #endregion

    #region save and load between scenes

    public void SaveStats(Character[] players)
    {
        //save stats for players
        foreach (Character player in players)
        {
            //health and ammos
            savedStats = new SavesBetweenScenes();
            if (saveHealth && player.GetSavedComponent<HealthComponent>()) savedStats.CurrentHealth = player.GetSavedComponent<HealthComponent>().CurrentHealth;
            if (saveAmmo && player.GetSavedComponent<AdvancedWeaponComponent>()) savedStats.CurrentAmmos = new Dictionary<string, int>(player.GetSavedComponent<AdvancedWeaponComponent>().CurrentAmmos_NotSafe);

            //perks
            if (savePerks && player.GetSavedComponent<PerksComponent>()) savedStats.EquippedPerk = player.GetSavedComponent<PerksComponent>().EquippedPerk;

            //foreach weapon save prefab
            if (saveWeapons && player.GetSavedComponent<WeaponComponent>())
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

            //save index equipped weapon
            if (saveIndexEquippedWeapon && player.GetSavedComponent<WeaponComponent>())
                savedStats.IndexEquippedWeapon = player.GetSavedComponent<WeaponComponent>().IndexEquippedWeapon;
        }
    }

    public void LoadStats()
    {
        if (savedStats == null)
            return;

        foreach (Character player in FindObjectsOfType<Character>())
        {
            //foreach player, load stats
            if (player.CharacterType == Character.ECharacterType.Player)
            {
                //health, ammos and perks
                if (saveHealth && player.GetSavedComponent<HealthComponent>()) player.GetSavedComponent<HealthComponent>().CurrentHealth = savedStats.CurrentHealth;
                if (saveAmmo && player.GetSavedComponent<AdvancedWeaponComponent>()) player.GetSavedComponent<AdvancedWeaponComponent>().CurrentAmmos_NotSafe = new Dictionary<string, int>(savedStats.CurrentAmmos);
                if (savePerks && player.GetSavedComponent<PerksComponent>()) player.GetSavedComponent<PerksComponent>().AddPerk(savedStats.EquippedPerk);

                //weapons and index equipped weapon
                if (player.GetSavedComponent<WeaponComponent>())
                    StartCoroutine(LoadWeaponsCoroutine(player));
            }
        }
    }

    public void ClearStats()
    {
        //clear stats
        savedStats = null;
    }

    IEnumerator LoadWeaponsCoroutine(Character player)
    {
        //wait one frame (so weapon component will set number of weapons)
        yield return null;

        if (player && player.GetSavedComponent<WeaponComponent>())
        {
            //instantiate and equip saved weapons
            if (saveWeapons && savedStats.WeaponsPrefabs != null)
            {
                int length = player.GetSavedComponent<WeaponComponent>().CurrentWeapons.Length;
                for (int i = 0; i < length; i++)
                {
                    if (i < savedStats.WeaponsPrefabs.Length)
                    {
                        if (savedStats.WeaponsPrefabs[i])
                        {
                            WeaponBASE instantiatedWeapon = Instantiate(savedStats.WeaponsPrefabs[i]);
                            instantiatedWeapon.WeaponPrefab = savedStats.WeaponsPrefabs[i];

                            //set 0 ammo on pick, because we have already its ammo (we are transporting this weapon from previous scene)
                            if (instantiatedWeapon is WeaponRange rangeWeapon)
                                rangeWeapon.AmmoOnPick = 0;

                            player.GetSavedComponent<WeaponComponent>().PickWeapon(instantiatedWeapon);
                        }
                    }
                    else
                        break;
                }
            }

            //load saved index weapon
            if (saveIndexEquippedWeapon)
                player.GetSavedComponent<WeaponComponent>().SwitchWeaponTo(savedStats.IndexEquippedWeapon);
        }
    }

    public static bool CanLoadDefaultWeapons()
    {
        //check if save weapons and there is a save (it's not the first level/lobby). If not, load default weapons
        return (instance && instance.saveWeapons && instance.savedStats != null) == false;
    }

    public static bool CanLoadDefaultAmmos()
    {
        //check if save ammo and there is a save (it's not the first level/lobby). If not, load default ammo
        return (instance && instance.saveAmmo && instance.savedStats != null) == false;
    }

    #endregion

    #region save and load checkpoint

    public void SaveCheckpoint(ReachedCheckpoint checkpoint)
    {
        if (checkpoint == null)
            return;

        //save debug
        currentCheckpoint = checkpoint.CheckpointNumber;

        //get player in level manager
        Character player = GameManager.instance && GameManager.instance.levelManager && GameManager.instance.levelManager.Players != null && GameManager.instance.levelManager.Players.Count > 0 ?
            GameManager.instance.levelManager.Players[0] : null;

        //save reached checkpoint
        reachedCheckpoint = new SavesCheckpoint();
        reachedCheckpoint.CheckpointNumber = checkpoint.CheckpointNumber;
        reachedCheckpoint.CurrentHealth = checkpoint.SaveHealth && player && player.GetSavedComponent<HealthComponent>() ? player.GetSavedComponent<HealthComponent>().CurrentHealth : -1;
        reachedCheckpoint.CurrentAmmos = checkpoint.SaveAmmo && player && player.GetSavedComponent<AdvancedWeaponComponent>() ? new Dictionary<string, int>(player.GetSavedComponent<AdvancedWeaponComponent>().CurrentAmmos_NotSafe) : null;
    }

    public void SaveCheckpoint(int checkpointNumber)
    {
        //find checkpoint with that number, and save
        SaveCheckpoint(FindObjectsOfType<ReachedCheckpoint>().Where(x => x.CheckpointNumber == checkpointNumber).FirstOrDefault());
    }

    public void LoadCheckpoint()
    {
        //do only if checkpoint is saved
        if (reachedCheckpoint == null)
            return;

        //find checkpoint with reached level
        foreach (ReachedCheckpoint checkpoint in FindObjectsOfType<ReachedCheckpoint>())
        {
            if (checkpoint.CheckpointNumber == reachedCheckpoint.CheckpointNumber)
            {
                //load it
                checkpoint.LoadCheckpoint();

                //find player
                Character player = FindObjectsOfType<Character>().Where(x => x.CharacterType == Character.ECharacterType.Player).FirstOrDefault();

                //load also health
                if (reachedCheckpoint.CurrentHealth > 0)
                {
                    if (player.GetSavedComponent<HealthComponent>()) player.GetSavedComponent<HealthComponent>().CurrentHealth = reachedCheckpoint.CurrentHealth;
                }
                // and ammo
                if (reachedCheckpoint.CurrentAmmos != null)
                {
                    if (player.GetSavedComponent<AdvancedWeaponComponent>()) player.GetSavedComponent<AdvancedWeaponComponent>().CurrentAmmos_NotSafe = new Dictionary<string, int>(reachedCheckpoint.CurrentAmmos);
                }

                break;
            }
        }
    }

    public void ClearCheckpoint()
    {
        //clear checkpoint
        reachedCheckpoint = null;
    }

    public static bool CanLoadDefaultAmmos_Checkpoint()
    {
        //check if saved checkpoint and saved also ammo. If not, load default ammo
        return (instance && instance.reachedCheckpoint != null && instance.reachedCheckpoint.CurrentAmmos != null) == false;
    }

    #endregion
}

[System.Serializable]
public class SavesBetweenScenes
{
    public float CurrentHealth;
    public Dictionary<string, int> CurrentAmmos;
    public PerkData EquippedPerk;
    public WeaponBASE[] WeaponsPrefabs;
    public int IndexEquippedWeapon;
}

[System.Serializable]
public class SavesCheckpoint
{
    public int CheckpointNumber;
    public float CurrentHealth;
    public Dictionary<string, int> CurrentAmmos;
}

#region interface

//interface to know where to save (key), type of the class and to reset generic class
public interface ISaveClass 
{ 
    string key { get; } 
    System.Type type { get; }

    ISaveClass GetEmptyClass();
}

#endregion

[System.Serializable]
public class SaveClassMoney : ISaveClass
{
    public string key => "Money";
    public System.Type type => typeof(SaveClassMoney);
    public ISaveClass GetEmptyClass() => new SaveClassMoney();

    //save money
    public int Money;
}

[System.Serializable]
public class SaveClassBoughtElements : ISaveClass
{
    public string key => "BoughtWeapons";
    public System.Type type => typeof(SaveClassBoughtElements);
    public ISaveClass GetEmptyClass() => new SaveClassBoughtElements();

    //save bought weapons and perks
    public List<ISellable> BoughtElements { get { 
            //return a list with weapons and perks together
            List<ISellable> sellables = new List<ISellable>(); 
            if (BoughtWeapons != null) sellables.AddRange(BoughtWeapons); 
            if (BoughtPerks != null) sellables.AddRange(BoughtPerks); 
            return sellables; 
        }  set {
            //set weapons
            BoughtWeapons = new List<WeaponBASE>();
            foreach (ISellable sellable in value)
                if (sellable is WeaponBASE)
                    BoughtWeapons.Add(sellable as WeaponBASE);

            //and perks
            BoughtPerks = new List<PerkData>();
            foreach (ISellable sellable in value)
                if (sellable is PerkData)
                    BoughtPerks.Add(sellable as PerkData); } }
    public List<WeaponBASE> BoughtWeapons;
    public List<PerkData> BoughtPerks;
}

[System.Serializable]
public class SaveClassLevelReached : ISaveClass
{
    public string key => "LevelReached";
    public System.Type type => typeof(SaveClassLevelReached);
    public ISaveClass GetEmptyClass() => new SaveClassLevelReached();

    //save level reached
    public int LevelReached;
}