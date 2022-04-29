using redd096.GameTopDown2D;
using UnityEngine;

public class CoinPickUp : PickUpBASE
{
    [Header("Coin")]
    [SerializeField] int coinsToGive = 10;

    public override void PickUp()
    {
        //check if hit has component
        WalletComponent whoHitComponent = whoHit.GetSavedComponent<WalletComponent>();
        if (whoHitComponent)
        {
            //give money
            whoHitComponent.Money += coinsToGive;
            OnPick();
        }
    }
}
