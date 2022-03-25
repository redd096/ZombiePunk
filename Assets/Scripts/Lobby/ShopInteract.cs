using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096;
using UnityEngine.UI;

public class ShopInteract : MonoBehaviour, IInteractable
{
    [Header("Canvas to Show")]
    [SerializeField] GameObject shopMenu = default;

    [Header("UI")]
    [SerializeField] Text currencyText = default;
    [SerializeField] string stringCurrency = "MONEY: ";
    [SerializeField] Color colorWhenTooExpensive = Color.red;
    [SerializeField] WeaponButtonShop[] weaponButtons = default;
    [SerializeField] WeaponBASE[] weaponsToBuy = default;

    InteractComponent whoIsInteracting;
    Redd096Main mainInteracting;
    List<WeaponBASE> alreadyBoughtWeapons = new List<WeaponBASE>();

    void Start()
    {
        //deactive menu at start
        if (shopMenu)
            shopMenu.SetActive(false);

        //set every button in shop
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            //if there are no more weapons, stop
            if (i >= weaponsToBuy.Length)
            {
                break;
            }

            if (weaponButtons[i] && weaponsToBuy[i])
            {
                //set name and price text
                if (weaponButtons[i].nameText) weaponButtons[i].nameText.text = weaponsToBuy[i].WeaponName;
                if (weaponButtons[i].priceText) weaponButtons[i].priceText.text = weaponsToBuy[i].WeaponPrice.ToString();

                //set button event and image
                if (weaponButtons[i].button)
                {
                    weaponButtons[i].button.GetComponent<Image>().sprite = weaponsToBuy[i].WeaponSprite;
                    int index = i;  //for some reason if use "i" it save every function with weaponsButtons.Length, so we must to create another variable
                    weaponButtons[i].button.onClick.AddListener(() => OnClickButton(index));
                }
            }
        }
    }

    void OnClickButton(int index)
    {
        //be sure there is a weapon at this index
        if (index < weaponsToBuy.Length && weaponsToBuy[index] != null)
        {
            //be sure player has enough money, and this weapons is not already bought (teorically is not possible, because the button is deactivated when update UI)
            int currentMoney = mainInteracting && mainInteracting.GetSavedComponent<WalletComponent>() ? mainInteracting.GetSavedComponent<WalletComponent>().Money : 0;
            if (currentMoney >= weaponsToBuy[index].WeaponPrice && alreadyBoughtWeapons.Contains(weaponsToBuy[index]) == false)
            {
                //add to already bought weapons
                alreadyBoughtWeapons.Add(weaponsToBuy[index]);

                //save
                SaveClassBoughtWeapons saveClass = GameManager.instance && GameManager.instance.Load<SaveClassBoughtWeapons>() != null ? GameManager.instance.Load<SaveClassBoughtWeapons>() : new SaveClassBoughtWeapons();
                saveClass.BoughtWeapons = alreadyBoughtWeapons;
                if (GameManager.instance) GameManager.instance.Save(saveClass);

                //remove money
                if (mainInteracting && mainInteracting.GetSavedComponent<WalletComponent>())
                    mainInteracting.GetSavedComponent<WalletComponent>().Money -= weaponsToBuy[index].WeaponPrice;

                //pick weapon
                if (mainInteracting && mainInteracting.GetSavedComponent<WeaponComponent>())
                    mainInteracting.GetSavedComponent<WeaponComponent>().PickWeaponPrefab(weaponsToBuy[index]);

                //save stats for next scene
                if (GameManager.instance && GameManager.instance.levelManager)
                    GameManager.instance.SaveStats(GameManager.instance.levelManager.Players.ToArray());

                //update UI
                UpdateUIShop();
            }
        }

        //TODO
        //V andranno salvate le armi comprate (player prefs o json?)
        //V quelle già comprate non dovranno essere interagili nello shop
        //- MA quelle già comprate dovranno essere disponibili invece in un altro script, per l'inventario

        //V bisognerà inserire la currency (sempre salvata in player prefs o json) e dev'essere sottratta al player
        //- NB andranno creati anche dei prefab pickable per raccogliere currency e aggiungerla al nostro salvataggio

        //V quando il player muore, le armi correntemente equipaggiate, andranno rimosse da quelle salvate.
        //V quindi saranno di nuovo interagibili nello shop e spariranno dall'inventario
        //V NB solo quelle equipaggiate, non le altre nell'inventario
        //V p.s. mettere magari una booleana per decidere se perdere le armi equipaggiate o no

        //NB PER L'INVENTARIO
        //- che quindi o si aggiorna ad ogni update del salvataggio, o si genera quando lo si apre e non allo start
        //- perché se si compra nello shop, poi dev'essere aggiornato

        //- un'altra cosa da fare è fixare il tasto per uscire, perché ora chiude il menu, ma essendo lo stesso per mettere in pausa, si mette in pausa appena torna nel NormalState
        //forse conviene fare come nel PauseState, che non cambia stato quando preme il tasto, ma quando timeScale torna maggiore di 0, così è già passato il frame.
        //- NB che se si fa così, bisogna rimuovere la condition dal NormalState che se TimeScale è 0 allora va in pausa.
        //- e bisogna fixare sta cosa anche per il MapInteract, e smetterla di aprire il menu di pausa
    }

    void UpdateUIShop()
    {
        //check current money
        int currentMoney = mainInteracting && mainInteracting.GetSavedComponent<WalletComponent>() ? mainInteracting.GetSavedComponent<WalletComponent>().Money : 0;

        //set currency text
        if (currencyText)
            currencyText.text = stringCurrency + currentMoney.ToString();

        //load already bought weapons
        SaveClassBoughtWeapons saveClass = GameManager.instance ? GameManager.instance.Load<SaveClassBoughtWeapons>() : null;
        alreadyBoughtWeapons = (saveClass != null && saveClass.BoughtWeapons != null) ? saveClass.BoughtWeapons : new List<WeaponBASE>();

        //check every button
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            //initialize to save default text colors (will be saved only first time)
            if (weaponButtons[i]) weaponButtons[i].Init();

            //if this button has no weapon, do not show it (ignore also if button is not setted)
            if (i >= weaponsToBuy.Length || weaponsToBuy[i] == null || weaponButtons[i] == null)
            {
                if (weaponButtons[i]) weaponButtons[i].gameObject.SetActive(false);
                continue;
            }

            //if already bought, deactive button
            if (alreadyBoughtWeapons.Contains(weaponsToBuy[i]))
            {
                Color disabledColor = weaponButtons[i].button ? weaponButtons[i].button.colors.disabledColor : Color.grey;
                if (weaponButtons[i].button) weaponButtons[i].button.interactable = false;
                if (weaponButtons[i].nameText) weaponButtons[i].nameText.color = disabledColor;
                if (weaponButtons[i].priceText) weaponButtons[i].priceText.color = disabledColor;
            }
            //else check price to set interactable or not
            else
            {
                bool canBuy = currentMoney >= weaponsToBuy[i].WeaponPrice;
                if (weaponButtons[i].button) weaponButtons[i].button.interactable = canBuy;
                if (weaponButtons[i].nameText) weaponButtons[i].nameText.color = canBuy ? weaponButtons[i].GetDefaultNameTextColor() : colorWhenTooExpensive;
                if (weaponButtons[i].priceText) weaponButtons[i].priceText.color = canBuy ? weaponButtons[i].GetDefaultPriceTextColor() : colorWhenTooExpensive;
            }
        }
    }

    public void Interact(InteractComponent whoInteract)
    {
        //if interact a state machine, set LobbyMenu State
        StateMachineRedd096 sm = whoInteract.GetComponentInChildren<StateMachineRedd096>();
        if (sm)
        {
            sm.SetState(2);
        }

        //save references
        whoIsInteracting = whoInteract;
        if (whoIsInteracting) mainInteracting = whoIsInteracting.GetComponent<Redd096Main>();

        //register to event to know when player press input to close
        whoInteract.inputEventForStateMachines += OnInputPressToCloseMenu;

        //set which buttons are interactable and money text
        UpdateUIShop();

        //open shop menu
        GameManager.instance.uiManager.OpenMenu(shopMenu, true);

        //show cursor
        SceneLoader.instance.LockMouse(CursorLockMode.None);
    }

    void OnInputPressToCloseMenu()
    {
        //remove event
        if (whoIsInteracting)
            whoIsInteracting.inputEventForStateMachines -= OnInputPressToCloseMenu;

        //close shop menu
        GameManager.instance.uiManager.OpenMenu(shopMenu, false);

        //hide cursor
        if (SceneLoader.instance.ChangeCursorLockMode)
            SceneLoader.instance.LockMouse(SceneLoader.instance.LockModeOnResume);

        //if interact a state machine, re-set Normal State
        StateMachineRedd096 sm = whoIsInteracting.GetComponentInChildren<StateMachineRedd096>();
        if (sm)
        {
            sm.SetState(0);
        }
    }

    /// <summary>
    /// Can call this from button in UI or from another script
    /// </summary>
    public void CloseShop()
    {
        OnInputPressToCloseMenu();
    }

    /// <summary>
    /// Delete save Bought Weapons
    /// </summary>
    public void ClearBoughtWeapons()
    {
        if (GameManager.instance) GameManager.instance.ClearSave<SaveClassBoughtWeapons>();
        UpdateUIShop();
    }
}
