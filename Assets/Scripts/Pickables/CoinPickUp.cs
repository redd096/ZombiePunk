using redd096.GameTopDown2D;
using UnityEngine;

public class CoinPickUp : PickUpBASE
{
    [Header("Coin")]
    [SerializeField] int coinsToGive = 10;

    WalletComponent whoHitComponent;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        //save component from magnet or hit
        if (whoHitComponent == null)
        {
            if (player && whoHit == null)
                whoHitComponent = player.GetComponent<WalletComponent>();
            else if (whoHit)
                whoHitComponent = whoHit.GetSavedComponent<WalletComponent>();
        }
    }

    public override void PickUp()
    {
        //if can pick
        if (CanPickUp())
        {
            //give money
            whoHitComponent.Money += coinsToGive;
            OnPick();
        }
        //else fail pick
        else
        {
            OnFailPick();
        }
    }

    protected override bool CanPickUp()
    {
        //there is wallet component
        return whoHitComponent != null;
    }
}
