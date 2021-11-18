using UnityEngine;

public class WeaponChangeOrderOnRotateFeedback : MonoBehaviour
{
    [Header("Sprites - default get in children")]
    [SerializeField] SpriteRenderer[] spritesToUse = default;
    [SerializeField] int layerWhenLookingRight = 1;
    [SerializeField] int layerWhenLookingLeft = -1;

    WeaponBASE weapon;
    AimComponent owner;

    void OnEnable()
    {
        //get referemces
        weapon = GetComponent<WeaponBASE>();
        if (spritesToUse == null || spritesToUse.Length <= 0) spritesToUse = GetComponentsInChildren<SpriteRenderer>();

        //add events
        if(weapon)
        {
            weapon.onPickWeapon += OnPickWeapon;
            weapon.onDropWeapon += OnDropWeapon;

            //if there is an owner
            if(weapon.Owner)
            {
                //and has aim component, add its events too
                owner = weapon.Owner.GetSavedComponent<AimComponent>();
                if(owner)
                {
                    owner.onChangeAimDirection += OnChangeAimDirection;
                    UpdateOrderInLayer(owner.IsLookingRight);               //update order
                }
            }
        }
    }

    void OnDisable()
    {
        //remove events
        if (weapon)
        {
            weapon.onPickWeapon -= OnPickWeapon;
            weapon.onDropWeapon -= OnDropWeapon;
        }

        if(owner)
        {
            owner.onChangeAimDirection -= OnChangeAimDirection;
        }
    }

    void UpdateOrderInLayer(bool isLookingRight)
    {
        //foreach sprite, update order
        foreach(SpriteRenderer sprite in spritesToUse)
        {
            sprite.sortingOrder = isLookingRight ? layerWhenLookingRight : layerWhenLookingLeft;
        }
    }

    #region events

    void OnPickWeapon()
    {
        //register to owner events
        if (weapon.Owner)
        {
            owner = weapon.Owner.GetSavedComponent<AimComponent>();
            if(owner)
            {
                owner.onChangeAimDirection += OnChangeAimDirection;
                UpdateOrderInLayer(owner.IsLookingRight);
            }
        }
    }

    void OnDropWeapon()
    {
        //remove events from owner
        if(owner)
        {
            owner.onChangeAimDirection -= OnChangeAimDirection;
        }
    }

    void OnChangeAimDirection(bool isLookingRight)
    {
        //update order in layer
        UpdateOrderInLayer(isLookingRight);
    }

    #endregion
}
