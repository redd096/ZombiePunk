using UnityEngine;

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
            SaveClassMoney saveClass = GameManager.instance && GameManager.instance.Load<SaveClassMoney>() != null ? GameManager.instance.Load<SaveClassMoney>() : new SaveClassMoney();
            saveClass.Money = money;
            if (GameManager.instance) GameManager.instance.Save(saveClass);

            //call event and update UI
            onChangeMoney?.Invoke(money);
            if (GameManager.instance && GameManager.instance.uiManager) GameManager.instance.uiManager.SetCurrencyText(money);
        }
    }

    //events
    public System.Action<int> onChangeMoney { get; set; }

    void Start()
    {
        //load money at start
        SaveClassMoney saveClass = GameManager.instance ? GameManager.instance.Load<SaveClassMoney>() : null;
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
