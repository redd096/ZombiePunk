using UnityEngine;
using redd096;

[System.Serializable]
public class SaveClassMoney
{
    public int Money;
}

public class WalletComponent : MonoBehaviour
{
    [SerializeField] int money = 0;

    public int Money
    {
        get
        {
            return money;
        }
        set
        {
            money = value;

            //save
            SaveClassMoney saveClass = new SaveClassMoney();
            saveClass.Money = money;
            SaveLoadJSON.Save(MONEY_SAVENAME, saveClass);

            //call event and update UI
            onChangeMoney?.Invoke(money);
            if (GameManager.instance && GameManager.instance.uiManager) GameManager.instance.uiManager.SetCurrencyText(money);
        }
    }

    //save and load money
    const string MONEY_SAVENAME = "Money";

    //events
    public System.Action<int> onChangeMoney { get; set; }

    void Start()
    {
        //load money at start
        SaveClassMoney saveClass = SaveLoadJSON.Load<SaveClassMoney>(MONEY_SAVENAME);
        if (saveClass != null)
        {
            money = saveClass.Money;

            //call event and update UI
            onChangeMoney?.Invoke(money);
        }

        //update UI
        if (GameManager.instance && GameManager.instance.uiManager)
            GameManager.instance.uiManager.SetCurrencyText(money);
    }
}
