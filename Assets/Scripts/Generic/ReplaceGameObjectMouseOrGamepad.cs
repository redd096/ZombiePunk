using UnityEngine;
using UnityEngine.InputSystem;

public class ReplaceGameObjectMouseOrGamepad : MonoBehaviour
{
    [Header("change object if use mouse or gamepad")]
    [SerializeField] string mouseSchemeName = "KeyboardAndMouse";
    [SerializeField] GameObject objectMouse = default;
    [SerializeField] GameObject objectGamepad = default;

    PlayerInput playerInput;
    bool usingMouse;

    void Start()
    {
        //get references
        if (GameManager.instance && GameManager.instance.levelManager && GameManager.instance.levelManager.Players.Count > 0)
            playerInput = GameManager.instance.levelManager.Players[0].GetSavedComponent<PlayerInput>();

        //set gameObject
        ReplaceGameObjects();
    }

    void Update()
    {
        //if change device, replace gameObjects
        if (playerInput && IsUsingMouse() != usingMouse)
        {
            ReplaceGameObjects();
        }
    }

    bool IsUsingMouse()
    {
        //return if current control scheme is mouse scheme
        return playerInput ? playerInput.currentControlScheme == mouseSchemeName : true;
    }

    void ReplaceGameObjects()
    {
        //set if using mouse or gamepad
        if (GameManager.instance.levelManager.Players.Count > 0)
        {
            usingMouse = IsUsingMouse();
        }

        //active or deactive objects
        if (objectMouse)
            objectMouse.SetActive(usingMouse);
        if (objectGamepad)
            objectGamepad.SetActive(!usingMouse);
    }
}
