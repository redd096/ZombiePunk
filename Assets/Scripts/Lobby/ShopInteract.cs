using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096;
using UnityEngine.UI;

[System.Serializable]
public class SaveClass
{
    public List<WeaponBASE> BoughtWeapons;
}

public class ShopInteract : MonoBehaviour, IInteractable
{
    [Header("Canvas to Show")]
    [SerializeField] GameObject shopMenu = default;

    [Header("UI")]
    [SerializeField] Text currencyText = default;
    [SerializeField] WeaponButtonShop[] weaponButtons = default;
    [SerializeField] WeaponBASE[] weaponsToBuy = default;

    const string SAVENAME = "BoughtWeapons";

    InteractComponent whoIsInteracting;
    SaveClass saveClass;

    void Start()
    {
        //load already bought weapons
        saveClass = SaveLoadJSON.Load<SaveClass>(SAVENAME);
        List<WeaponBASE> alreadyBoughtWeapons = saveClass != null && saveClass.BoughtWeapons != null ? saveClass.BoughtWeapons : new List<WeaponBASE>();    //never null

        for (int i = 0; i < weaponButtons.Length; i++)
        {
            //if there are no more weapons, stop
            if (i >= weaponsToBuy.Length)
                break;

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

                //if already bought
                if (alreadyBoughtWeapons.Contains(weaponsToBuy[i]))
                {
                    Color disabledColor = weaponButtons[i].button ? weaponButtons[i].button.colors.disabledColor : Color.grey;
                    if (weaponButtons[i].button) weaponButtons[i].button.interactable = false;
                    if (weaponButtons[i].nameText) weaponButtons[i].nameText.color = disabledColor;
                    if (weaponButtons[i].priceText) weaponButtons[i].priceText.color = disabledColor;
                }

                //TODO
                //bisogna disattivare completamente quelli non comprabili perché già presi
                //bisogna scrivere in rosso il prezzo e disattivare il button di quelli invece troppo costosi

                //bisogna settare il currencyText con i soldi correnti
            }
        }
    }

    void OnClickButton(int index)
    {
        //get saved weapons and add this one
        List<WeaponBASE> alreadyBoughtWeapons = saveClass != null && saveClass.BoughtWeapons != null ? saveClass.BoughtWeapons : new List<WeaponBASE>();    //never null
        if (index < weaponsToBuy.Length && weaponsToBuy[index] != null)
            alreadyBoughtWeapons.Add(weaponsToBuy[index]);

        //be sure to have a class to save
        if (saveClass == null)
            saveClass = new SaveClass();

        //set saved weapons updated
        saveClass.BoughtWeapons = alreadyBoughtWeapons;

        //save in file
        SaveLoadJSON.Save(SAVENAME, saveClass);

        //TODO
        //andranno salvate le armi comprate (player prefs o json?)
        //quelle già comprate non dovranno essere interagili nello shop
        //MA quelle già comprate dovranno essere disponibili invece in un altro script, per l'inventario

        //bisognerà inserire la currency (sempre salvata in player prefs o json) e dev'essere sottratta al player
        //NB andranno creati anche dei prefab pickable per raccogliere currency e aggiungerla al nostro salvataggio

        //quando il player muore, le armi correntemente equipaggiate, andranno rimosse da quelle salvate.
        //quindi saranno di nuovo interagibili nello shop e spariranno dall'inventario
        //NB solo quelle equipaggiate, non le altre nell'inventario
        //p.s. mettere magari una booleana per decidere se perdere le armi equipaggiate o no

        //NB PER L'INVENTARIO
        //che quindi o si aggiorna ad ogni update del salvataggio, o si genera quando lo si apre e non allo start
        //perché se si compra nello shop, poi dev'essere aggiornato
    }

    public void Interact(InteractComponent whoInteract)
    {
        //if interact a state machine, set LobbyMenu State
        StateMachineRedd096 sm = whoInteract.GetComponentInChildren<StateMachineRedd096>();
        if (sm)
        {
            sm.SetState(2);
        }

        //open shop menu
        GameManager.instance.uiManager.OpenMenu(shopMenu, true);

        //register to event to know when player press input to close
        whoIsInteracting = whoInteract;
        whoInteract.inputEventForStateMachines += OnInputPressToCloseMenu;
    }

    void OnInputPressToCloseMenu()
    {
        //remove event
        if (whoIsInteracting)
            whoIsInteracting.inputEventForStateMachines -= OnInputPressToCloseMenu;

        //close shop menu
        GameManager.instance.uiManager.OpenMenu(shopMenu, false);

        //if interact a state machine, re-set Normal State
        StateMachineRedd096 sm = whoIsInteracting.GetComponentInChildren<StateMachineRedd096>();
        if (sm)
        {
            sm.SetState(0);
        }
    }
}
