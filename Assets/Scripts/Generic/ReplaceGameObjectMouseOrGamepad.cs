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
        playerInput = FindObjectOfType<PlayerInput>();

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
        usingMouse = IsUsingMouse();

        //active or deactive objects
        if (objectMouse)
            objectMouse.SetActive(usingMouse);
        if (objectGamepad)
            objectGamepad.SetActive(!usingMouse);
    }
}
