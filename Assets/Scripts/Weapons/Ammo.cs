using System.Collections;
using UnityEngine;
using redd096;

public class Ammo : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] string ammoType = "GunAmmo";
    [SerializeField] int quantity = 1;

    [Header("Destroy when instantiated - 0 = no destroy")]
    [SerializeField] float timeBeforeDestroy = 0;

    public string AmmoType => ammoType;

    //events
    public System.Action onPickAmmo { get; set; }
    public System.Action onFailPickAmmo { get; set; }

    Character whoHit;
    AdvancedWeaponComponent whoHitWeaponComponent;
    bool alreadyUsed;

    void OnEnable()
    {
        //reset vars
        alreadyUsed = false;

        //if there, start auto destruction timer
        if (timeBeforeDestroy > 0)
            StartCoroutine(AutoDestruction());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (alreadyUsed)
            return;

        //if hitted by player
        whoHit = collision.gameObject.GetComponentInParent<Character>();
        if (whoHit && whoHit.CharacterType == Character.ECharacterType.Player)
        {
            //and player has weapon component
            whoHitWeaponComponent = whoHit.GetSavedComponent<AdvancedWeaponComponent>();
            if (whoHitWeaponComponent)
            {
                //if full of ammo, can't pick, call fail event
                if (whoHitWeaponComponent.IsFullOfAmmo(ammoType))
                {
                    onFailPickAmmo?.Invoke();
                }
                //else, pick and add quantity
                else
                {
                    whoHitWeaponComponent.AddAmmo(ammoType, quantity);

                    //call event
                    onPickAmmo?.Invoke();

                    //destroy this gameObject
                    alreadyUsed = true;
                    Destroy(gameObject);
                }
            }
        }
    }

    IEnumerator AutoDestruction()
    {
        //wait, then destroy
        yield return new WaitForSeconds(timeBeforeDestroy);
        alreadyUsed = true;
        Destroy(gameObject);
    }
}
