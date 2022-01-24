using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using redd096;

public class CheatMode : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null)
            return;

        //F1 enable/disable invincibility on this character
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Character selfCharacter = GetComponent<Character>();
            if (selfCharacter)
            {
                if (selfCharacter.GetSavedComponent<HealthComponent>()) selfCharacter.GetSavedComponent<HealthComponent>().Invincible = !selfCharacter.GetSavedComponent<HealthComponent>().Invincible;
            }
        }
        //F2 open/close every exit in this scene
        else if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            foreach (ExitInteractable exit in FindObjectsOfType<ExitInteractable>())
                exit.ForceExit();
        }
    }
}
