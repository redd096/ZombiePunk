using UnityEngine;
using UnityEngine.InputSystem;
using redd096;

public class CheatMode : MonoBehaviour
{
    [SerializeField] float timeLabel = 1.5f;
    [SerializeField] SpriteRenderer[] sprites = default;

    float timerShowLabel;

    void Start()
    {
        //set timer label and get references
        timerShowLabel = Time.time + timeLabel;
        if (sprites == null || sprites.Length <= 0) sprites = GetComponentsInChildren<SpriteRenderer>();

        //show if invincible at start
        HealthComponent selfHealthComponent = GetComponent<HealthComponent>();
        if (selfHealthComponent && selfHealthComponent.Invincible)
        {
            ColorSpritesWhenInvincible(true);
        }
    }

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
                if (selfCharacter.GetSavedComponent<HealthComponent>())
                {
                    selfCharacter.GetSavedComponent<HealthComponent>().Invincible = !selfCharacter.GetSavedComponent<HealthComponent>().Invincible;
                    ColorSpritesWhenInvincible(selfCharacter.GetSavedComponent<HealthComponent>().Invincible);
                }
            }
        }
        //F2 open/close every exit in this scene
        else if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            foreach (ExitInteractable exit in FindObjectsOfType<ExitInteractable>())
                exit.ForceExit();
        }
    }

    void OnGUI()
    {
        //only during timer
        if (Time.time > timerShowLabel)
            return;

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        //show label
        GUILayout.Label("Cheat Mode enabled");

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void ColorSpritesWhenInvincible(bool isInvincible)
    {
        //color sprite if invincible
        foreach (SpriteRenderer sprite in sprites)
            sprite.color = isInvincible ? Color.blue : Color.white;
    }
}
