using System.Collections.Generic;
using UnityEngine;
using redd096;
using NaughtyAttributes;

[DefaultExecutionOrder(-5)]
public class LevelManager : MonoBehaviour
{
    [Header("Elements in scene by default")]
    [ReadOnly] public HashSet<Character> Players = new HashSet<Character>();
    [ReadOnly] public HashSet<Character> Enemies = new HashSet<Character>();
    [ReadOnly] public HashSet<SpawnManager> SpawnManagers = new HashSet<SpawnManager>();
    [ReadOnly] public HashSet<ExitInteractable> Exits = new HashSet<ExitInteractable>();

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
            AddExit(exit);
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
        foreach (ExitInteractable exit in Exits)
        {
            RemoveExit(exit);
        }
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

    void AddExit(ExitInteractable exit)
    {
        //active it
        if (Exits.Contains(exit) == false)
        {
            exit.ActiveExit();

            //add to list
            Exits.Add(exit);
        }
    }

    void RemoveExit(ExitInteractable exit)
    {
        //deactivate it
        exit.DeactiveExit();
    }

    #endregion

    #region events

    void OnPlayerDie(HealthComponent whoDied)
    {
        //when a player die, set every player to state Null
        foreach (Character player in Players)
            player.GetComponentInChildren<StateMachineRedd096>().SetState(-1);

        //remove customizations
        if(GameManager.instance) GameManager.instance.ClearCustomizations();

        //and show end menu (and show cursor)
        if(GameManager.instance) GameManager.instance.uiManager.EndMenu(true);
        if(SceneLoader.instance) SceneLoader.instance.LockMouse(CursorLockMode.None);
    }

    #endregion
}
