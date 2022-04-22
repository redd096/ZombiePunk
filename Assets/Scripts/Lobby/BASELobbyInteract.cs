using UnityEngine;
using redd096.GameTopDown2D;
using redd096;

public abstract class BASELobbyInteract : MonoBehaviour, IInteractable
{
    [Header("Canvas to Show")]
    [SerializeField] GameObject menu = default;

    protected InteractComponent whoIsInteracting;
    protected Redd096Main mainInteracting;

    protected virtual void Start()
    {
        //deactive menu at start
        if (menu)
            menu.SetActive(false);
    }

    /// <summary>
    /// Can call this from button in UI or from another script
    /// </summary>
    public void CloseMenu()
    {
        OnInputPressToCloseMenu();
    }

    /// <summary>
    /// Called when someone interact with this object
    /// </summary>
    /// <param name="whoInteract"></param>
    public virtual void Interact(InteractComponent whoInteract)
    {
        //if interact a state machine, set LobbyMenu State
        StateMachineRedd096 sm = whoInteract.GetComponentInChildren<StateMachineRedd096>();
        if (sm)
        {
            sm.SetState("LobbyMenu State");
        }

        //save references
        whoIsInteracting = whoInteract;
        if (whoIsInteracting) mainInteracting = whoIsInteracting.GetComponent<Redd096Main>();

        //register to event to know when player press input to close
        whoInteract.inputEventForStateMachines += OnInputPressToCloseMenu;

        //set which buttons are interactable and money text
        UpdateUI();

        //open shop menu
        GameManager.instance.uiManager.OpenMenu(menu, true);

        //show cursor and deactive time
        SceneLoader.instance.LockMouse(CursorLockMode.None);
        Time.timeScale = 0;
    }

    /// <summary>
    /// When press Esc or the button to close menu
    /// </summary>
    protected virtual void OnInputPressToCloseMenu()
    {
        //remove event
        if (whoIsInteracting)
            whoIsInteracting.inputEventForStateMachines -= OnInputPressToCloseMenu;

        //close shop menu
        GameManager.instance.uiManager.OpenMenu(menu, false);

        //hide cursor
        if (SceneLoader.instance.ChangeCursorLockMode)
            SceneLoader.instance.LockMouse(SceneLoader.instance.LockModeOnResume);

        //reset time, so state machine will come back to Normal State
        Time.timeScale = 1;
    }

    protected abstract void UpdateUI();
}
