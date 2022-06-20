using redd096.GameTopDown2D;
using UnityEngine;

public class ComboPickUp : PickUpBASE
{
    [Header("Combo Bar")]
    [SerializeField] int comboPointToGive = 10;
    [Tooltip("Can pick when the bar is full?")] [SerializeField] bool canPickAlsoIfFull = false;

    ComboComponent whoHitComponent;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        //save component from magnet or hit
        if (whoHitComponent == null)
        {
            if (player && whoHit == null)
                whoHitComponent = player.GetComponent<ComboComponent>();
            else if (whoHit)
                whoHitComponent = whoHit.GetSavedComponent<ComboComponent>();
        }
    }

    public override void PickUp()
    {
        //if can pick
        if (CanPickUp())
        {
            //give money
            whoHitComponent.AddCombo(comboPointToGive);
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
        //there is component, and is not full or can pick anyway
        return whoHitComponent != null && (whoHitComponent.CurrentCombo < whoHitComponent.ComboToReach || canPickAlsoIfFull);
    }
}
