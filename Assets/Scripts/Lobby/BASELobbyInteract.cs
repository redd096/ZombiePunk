using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096;

public abstract class BASELobbyInteract : MonoBehaviour, IInteractable
{
    [Header("Canvas to Show")]
    [SerializeField] GameObject menu = default;
    [SerializeField] bool disableUIPlayer = true;
    [SerializeField] bool setTimeScaleAtZero = false;

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
        if (whoInteract == null)
            return;

        //save references
        whoIsInteracting = whoInteract;
        if (whoIsInteracting) mainInteracting = whoIsInteracting.GetComponent<Redd096Main>();

        //if interact a state machine, set LobbyMenu State
        StateMachineRedd096 sm = whoIsInteracting.GetComponentInChildren<StateMachineRedd096>();
        if (sm)
        {
            sm.SetState("LobbyMenu State");
        }

        //set time scale, if necessary
        if (setTimeScaleAtZero)
            Time.timeScale = 0;

        //show cursor
        SceneLoader.instance.LockMouse(CursorLockMode.None);

        //disable gameplay UI when show this canvas
        if (disableUIPlayer)
        {
            if (GameManager.instance && GameManager.instance.uiManager)
                GameManager.instance.uiManager.DisableUIWhenEnterInShop(true);
        }

        //set which buttons are interactable and money text
        UpdateUI();

        //open shop menu
        GameManager.instance.uiManager.OpenMenu(menu, true);

        //register to event to know when player press input to close
        whoIsInteracting.inputEventForStateMachines += OnInputPressToCloseMenu;
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

        //re-enable gameplay UI when hide this canvas
        if (disableUIPlayer)
        {
            if (GameManager.instance && GameManager.instance.uiManager)
                GameManager.instance.uiManager.DisableUIWhenEnterInShop(false);
        }

        //hide cursor
        if (SceneLoader.instance.ChangeCursorLockMode)
            SceneLoader.instance.LockMouse(SceneLoader.instance.LockModeOnResume);

        //set time scale, if necessary
        if (setTimeScaleAtZero)
            Time.timeScale = 1;

        //if interact a state machine, back to Normal State
        if (whoIsInteracting)
        {
            StateMachineRedd096 sm = whoIsInteracting.GetComponentInChildren<StateMachineRedd096>();
            StartCoroutine(ResetStatemachineCoroutine(sm)); //use coroutine to wait one frame, so if pressed same button to set Pause, will not move to PauseState
        }

        //remove ref
        whoIsInteracting = null;
        mainInteracting = null;
    }

    IEnumerator ResetStatemachineCoroutine(StateMachineRedd096 statemachine)
    {
        //wait one frame (so if pressed same button to set Pause, will not move to PauseState)
        yield return null;

        //back to NormalState
        if (statemachine)
        {
            statemachine.SetState("Normal State");
        }
    }

    protected abstract void UpdateUI();
}
