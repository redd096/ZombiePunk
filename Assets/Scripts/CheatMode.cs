using UnityEngine;
using UnityEngine.InputSystem;
using redd096.GameTopDown2D;
using redd096;

public class CheatMode : MonoBehaviour
{
    [Header("Necessary Components - default get from children")]
    [SerializeField] PlayerInput playerInput = default;
    [SerializeField] StateMachineRedd096 stateMachine = default;

    [Header("Cheat Settings")]
    [SerializeField] float timeLabel = 1.5f;
    [SerializeField] SpriteRenderer[] sprites = default;
    [SerializeField] WeaponBASE superWeapon = default;


    float timerShowLabel;

    void Start()
    {
        //set references
        if (playerInput == null) playerInput = GetComponentInChildren<PlayerInput>();
        if (stateMachine == null) stateMachine = GetComponentInChildren<StateMachineRedd096>();

        //show warnings if not found
        if (playerInput == null)
            Debug.LogWarning("Miss PlayerInput on " + name);
        else if (playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + name);
        if (stateMachine == null)
            Debug.LogWarning("Miss StateMachine on " + name);

        //settings

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
        if (stateMachine && stateMachine.GetCurrentState().Equals("Normal State", System.StringComparison.OrdinalIgnoreCase) == false)
            return;

        //F1 enable/disable invincibility on this character
        if (playerInput && playerInput.actions && playerInput.actions.FindAction("CHEAT - Invincible").triggered)
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
        else if (playerInput && playerInput.actions && playerInput.actions.FindAction("CHEAT - Exit").triggered)
        {
            foreach (ExitInteractable exit in FindObjectsOfType<ExitInteractable>())
                exit.ForceExit();
        }
        //F3 instantiate super weapon
        else if (playerInput && playerInput.actions && playerInput.actions.FindAction("CHEAT - Weapon").triggered)
        {
            if (superWeapon)
            {
                WeaponBASE weapon = Instantiate(superWeapon, transform.position, transform.rotation);
                weapon.WeaponPrefab = superWeapon;

                //equip as current equipped weapon
                WeaponComponent weaponComponent = GetComponent<WeaponComponent>();
                if (weaponComponent) weaponComponent.PickWeapon(weapon, weaponComponent.IndexEquippedWeapon);
            }
        }
#if UNITY_EDITOR
        //F10 to pause editor
        else if (Keyboard.current != null && Keyboard.current.f10Key.wasPressedThisFrame)
        {
            UnityEditor.EditorApplication.isPaused = true;
        }
        //F11 disable every canvas
        else if (Keyboard.current != null && Keyboard.current.f11Key.wasPressedThisFrame)
        {
            foreach (Canvas canvas in FindObjectsOfType<Canvas>())
                canvas.gameObject.SetActive(!canvas.gameObject.activeInHierarchy);
        }
#endif
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
