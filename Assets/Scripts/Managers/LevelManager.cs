using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;
using redd096.Attributes;

[DefaultExecutionOrder(-5)]
public class LevelManager : MonoBehaviour
{
    public enum ELevelState { Normal, Pause }

    [Header("Elements in scene by default")]
    [ReadOnly] public List<Character> Players = new List<Character>();
    [ReadOnly] public List<Character> Enemies = new List<Character>();
    [ReadOnly] public List<SpawnManager> SpawnManagers = new List<SpawnManager>();
    [ReadOnly] public List<ExitInteractable> Exits = new List<ExitInteractable>();

    [Header("Remove saved weapons on die?")]
    [SerializeField] bool removeSavedWeaponsOnDie = true;

    [Header("Time before load next scene - or open end menu when die")]
    [SerializeField] float timeBeforeNextScene = 1;
    [SerializeField] float timeBeforeOpenEndMenu = 1;

    [Header("DEBUG")]
    [ReadOnly] public ELevelState LevelState = ELevelState.Normal;

    void Awake()
    {
        //by default call resume game, to lock mouse
        if (SceneLoader.instance)
            SceneLoader.instance.ResumeGame();

        //if there is a save between scenes, load it
        if (SavesManager.instance)
            SavesManager.instance.LoadStats();

        //if there is saved a checkpoint, load it
        if (SavesManager.instance)
            SavesManager.instance.LoadCheckpoint();
    }

    void OnEnable()
    {
        //find every character in scene
        foreach(Character character in FindObjectsOfType<Character>())
        {
            if (character.CharacterType == Character.ECharacterType.Player) AddPlayer(character);
            else if (Enemies.Contains(character) == false) Enemies.Add(character);
        }

        //find every spawn manager in scene
        foreach(SpawnManager spawnManager in FindObjectsOfType<SpawnManager>())
        {
            if (SpawnManagers.Contains(spawnManager) == false)
                SpawnManagers.Add(spawnManager);
        }

        //find every exit in scene
        foreach(ExitInteractable exit in FindObjectsOfType<ExitInteractable>())
        {
            if (Exits.Contains(exit) == false)
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

    IEnumerator LoadNextSceneCoroutine(ExitInteractable exit)
    {
        //wait
        yield return new WaitForSeconds(timeBeforeNextScene);

        //save stats for next scene
        if (Players != null && SavesManager.instance)
            SavesManager.instance.SaveStats(Players.ToArray());

        //load next scene
        if (exit && SceneLoader.instance)
            SceneLoader.instance.LoadScene(exit.SceneToLoad);
    }

    IEnumerator OpenEndMenuCoroutine()
    {
        //wait
        yield return new WaitForSeconds(timeBeforeOpenEndMenu);

        //show end menu (and show cursor)
        if (GameManager.instance) GameManager.instance.uiManager.EndMenu(true);
        if (SceneLoader.instance) SceneLoader.instance.LockMouse(CursorLockMode.None);
    }

    #endregion

    #region public API

    public void OnInteractExit(ExitInteractable exit)
    {
        //start coroutine to load next level
        StartCoroutine(LoadNextSceneCoroutine(exit));
    }

    public void AddSpawnedExit(ExitInteractable exit)
    {
        //if an exit is spawned at runtime, add to the list
        if (Exits.Contains(exit) == false)
            Exits.Add(exit);
    }

    public void AddSpawnedSpawnManager(SpawnManager spawnManager)
    {
        //if a spawnManager is spawned at runtime, add to the list
        if (SpawnManagers.Contains(spawnManager) == false)
            SpawnManagers.Add(spawnManager);

        //be sure to add it also at every exit
        foreach (ExitInteractable exit in Exits)
            exit.AddSpawnManager(spawnManager);
    }

    #endregion

    #region events

    void OnPlayerDie(HealthComponent whoDied, Character whoHit)
    {
        //when a player die, set every player to state Null
        foreach (Character player in Players)
            player.GetComponentInChildren<StateMachineRedd096>().SetState(-1);

        ////OLD
        ////remove customizations and weapons
        //if (GameManager.instance)
        //{
        //    GameManager.instance.ClearCustomizations();
        //    GameManager.instance.ClearWeapons();
        //}

        //delete current weapons from saved already bought
        if (removeSavedWeaponsOnDie)
        {
            foreach (Character player in Players)
            {
                if (player && player.GetSavedComponent<WeaponComponent>())
                {
                    //load already bought weapons
                    SaveClassBoughtElements saveClass = SavesManager.instance ? SavesManager.instance.Load<SaveClassBoughtElements>() : new SaveClassBoughtElements();
                    List<ISellable> alreadyBoughtElements = (saveClass != null && saveClass.BoughtElements != null) ? saveClass.BoughtElements : new List<ISellable>();

                    //remove current weapons
                    foreach (WeaponBASE weapon in player.GetSavedComponent<WeaponComponent>().CurrentWeapons)
                    {
                        if (weapon && alreadyBoughtElements.Contains(weapon.WeaponPrefab))
                        {
                            alreadyBoughtElements.Remove(weapon.WeaponPrefab);
                        }
                    }

                    //save
                    saveClass.BoughtElements = alreadyBoughtElements;
                    if (SavesManager.instance) SavesManager.instance.Save(saveClass);
                }
            }
        }

        //open end menu after few seconds
        StartCoroutine(OpenEndMenuCoroutine());
    }

    #endregion
}
