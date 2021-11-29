using System.Collections.Generic;
using UnityEngine;
using redd096;
using NaughtyAttributes;

public class LevelManager : MonoBehaviour
{
    [ReadOnly] public List<Character> Players = new List<Character>();

    void OnEnable()
    {
        //find every player in scene and add to list
        foreach(Character character in FindObjectsOfType<Character>())
        {
            AddPlayer(character);
        }
    }

    void OnDisable()
    {
        //remove every player from the list
        foreach (Character character in new List<Character>(Players))
        {
            RemovePlayer(character);
        }
    }

    void AddPlayer(Character character)
    {
        //add only Player and not duplicate
        if (character.CharacterType != Character.ECharacterType.Player || Players.Contains(character))
            return;

        //add to the list and register to events
        Players.Add(character);
        if(character.GetSavedComponent<HealthComponent>())
            character.GetSavedComponent<HealthComponent>().onDie += OnPlayerDie;
    }

    public void RemovePlayer(Character character)
    {
        //remove only Player and only if inside the list
        if (character.CharacterType != Character.ECharacterType.Player || Players.Contains(character) == false)
            return;

        //remove from the list and unregister from events
        Players.Remove(character);
        if (character.GetSavedComponent<HealthComponent>())
            character.GetSavedComponent<HealthComponent>().onDie -= OnPlayerDie;
    }

    void OnPlayerDie()
    {
        //show end menu if player die
        GameManager.instance.uiManager.EndMenu(true);
    }
}
