using System.Collections.Generic;
using UnityEngine;
using redd096;
using NaughtyAttributes;

public class LevelManager : MonoBehaviour
{
    [ReadOnly] public List<Character> Players = new List<Character>();
    [ReadOnly] public List<Character> Enemies = new List<Character>();
    [ReadOnly] public List<ExitInteractable> Exits = new List<ExitInteractable>();

    void OnEnable()
    {
        //find every character in scene and add to lists
        foreach(Character character in FindObjectsOfType<Character>())
        {
            AddPlayer(character);
            AddEnemy(character);
        }

        //find every exit in scene and add to lists
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
        foreach (Character character in Enemies)
        {
            RemoveEnemy(character);
        }
        Enemies.Clear();

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
        //only Player and not duplicate
        if (character.CharacterType == Character.ECharacterType.Player && Players.Contains(character) == false)
        {
            //add to the list and register to events
            Players.Add(character);
            if (character.GetSavedComponent<HealthComponent>())
                character.GetSavedComponent<HealthComponent>().onDie += OnPlayerDie;
        }
    }

    void RemovePlayer(Character character)
    {
        //only if inside the list
        if (Players.Contains(character))
        {
            //unregister from events
            if (character.GetSavedComponent<HealthComponent>())
                character.GetSavedComponent<HealthComponent>().onDie -= OnPlayerDie;
        }
    }

    void AddEnemy(Character character)
    {
        //only AI and not duplicate
        if (character.CharacterType == Character.ECharacterType.AI && Enemies.Contains(character) == false)
        {
            //add to the list
            Enemies.Add(character);
        }
    }

    void RemoveEnemy(Character character)
    {
        //only if inside the list
        if (Enemies.Contains(character))
        {
            //remove from the list
            //Players.Remove(character);
        }
    }

    void AddExit(ExitInteractable exit)
    {
        //not duplicate
        if (Exits.Contains(exit) == false)
        {
            //add to list and active it
            Exits.Add(exit);
            exit.ActiveExit();
        }
    }

    void RemoveExit(ExitInteractable exit)
    {
        //only if inside the list
        if(Exits.Contains(exit))
        {
            //deactivate it
            exit.DeactiveExit();
        }
    }

    #endregion

    #region events

    void OnPlayerDie(HealthComponent whoDied)
    {
        //when a player die, set every player to state Null
        foreach (Character player in Players)
            player.GetComponentInChildren<StateMachineRedd096>().SetState(-1);

        //and show end menu
        GameManager.instance.uiManager.EndMenu(true);
    }

    #endregion
}
