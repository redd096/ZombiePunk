using System.Collections.Generic;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;
using redd096.Attributes;

[DefaultExecutionOrder(-5)]
public class LevelManager : MonoBehaviour
{
    [Header("Elements in scene by default")]
    [ReadOnly] public List<Character> Players = new List<Character>();
    [ReadOnly] public List<Character> Enemies = new List<Character>();
    [ReadOnly] public List<SpawnManager> SpawnManagers = new List<SpawnManager>();
    [ReadOnly] public List<ExitInteractable> Exits = new List<ExitInteractable>();

    [Header("Remove saved weapons on die?")]
    [SerializeField] bool removeSavedWeaponsOnDie = true;

    void Awake()
    {
        //by default call resume game, to lock mouse
        if (SceneLoader.instance)
            SceneLoader.instance.ResumeGame();
    }

    void OnEnable()
    {
        //find every character in scene
        foreach(Character character in FindObjectsOfType<Character>())
        {
            if (character.CharacterType == Character.ECharacterType.Player) AddPlayer(character);
            else Enemies.Add(character);
        }

        //find every spawn manager in scene
        foreach(SpawnManager spawnManager in FindObjectsOfType<SpawnManager>())
        {
            SpawnManagers.Add(spawnManager);
        }

        //find every exit in scene
        foreach(ExitInteractable exit in FindObjectsOfType<ExitInteractable>())
        {
            Exits.Add(exit);
        }
    }

    void OnDisable()
    {
        //remove every player from the list
        foreach (Character character in Players)
        {
            RemovePlayer(character);
        }
        Players.Clear();

        //remove every enemy from the list
        Enemies.Clear();

        //remove every spawn manager from the list
        SpawnManagers.Clear();

        //remove every exit from the list
        Exits.Clear();
    }

    #region private API

    void AddPlayer(Character character)
    {
        //register to events
        if (Players.Contains(character) == false)
        {
            if (character.GetSavedComponent<HealthComponent>())
                character.GetSavedComponent<HealthComponent>().onDie += OnPlayerDie;

            //add to the list
            Players.Add(character);
        }
    }

    void RemovePlayer(Character character)
    {
        //unregister from events
        if (character.GetSavedComponent<HealthComponent>())
            character.GetSavedComponent<HealthComponent>().onDie -= OnPlayerDie;
    }

    #endregion

    #region public API

    public void OnInteractExit(ExitInteractable exit)
    {
        //save stats for next scene
        GameManager.instance.SaveStats(Players.ToArray());

        //load next scene
        SceneLoader.instance.LoadScene(exit.SceneToLoad);
    }

    #endregion

    #region events

    void OnPlayerDie(HealthComponent whoDied)
    {
        //when a player die, set every player to state Null
        foreach (Character player in Players)
            player.GetComponentInChildren<StateMachineRedd096>().SetState(-1);

        //remove customizations and weapons
        if (GameManager.instance)
        {
            GameManager.instance.ClearCustomizations();
            GameManager.instance.ClearWeapons();
        }

        //delete current weapons from saved already bought
        if (removeSavedWeaponsOnDie)
        {
            foreach (Character player in Players)
            {
                if (player && player.GetSavedComponent<WeaponComponent>())
                {
                    //load already bought weapons
                    SaveClassBoughtWeapons saveClass = SavesManager.instance ? SavesManager.instance.Load<SaveClassBoughtWeapons>() : new SaveClassBoughtWeapons();
                    List<WeaponBASE> alreadyBoughtWeapons = (saveClass != null && saveClass.BoughtWeapons != null) ? saveClass.BoughtWeapons : new List<WeaponBASE>();

                    //remove current weapons
                    foreach (WeaponBASE weapon in player.GetSavedComponent<WeaponComponent>().CurrentWeapons)
                    {
                        if (weapon && alreadyBoughtWeapons.Contains(weapon.WeaponPrefab))
                        {
                            alreadyBoughtWeapons.Remove(weapon.WeaponPrefab);
                        }
                    }

                    //save
                    saveClass.BoughtWeapons = alreadyBoughtWeapons;
                    if (SavesManager.instance) SavesManager.instance.Save(saveClass);
                }
            }
        }

        //show end menu (and show cursor)
        if (GameManager.instance) GameManager.instance.uiManager.EndMenu(true);
        if (SceneLoader.instance) SceneLoader.instance.LockMouse(CursorLockMode.None);
    }

    #endregion
}
