using UnityEngine;
using Sirenix.OdinInspector;

public abstract class WeaponBASE : MonoBehaviour
{
    [Header("Weapon BASE")]
    public string WeaponName = "Weapon Name";
    public int WeaponPrice = 10;

    [Header("DEBUG")]
    [ReadOnly] public Redd096Main Owner;

    //events
    public System.Action onPickWeapon { get; set; }
    public System.Action onDropWeapon { get; set; }

    protected virtual void Update()
    {
        //follow Owner
        if (Owner)
            transform.position = Owner.transform.position;
    }

    #region public API

    /// <summary>
    /// Set owner to look at - and set parent
    /// </summary>
    /// <param name="character"></param>
    public void PickWeapon(Redd096Main character)
    {
        Owner = character;

        //call event
        onPickWeapon?.Invoke();
    }

    /// <summary>
    /// Remove owner and remove parent
    /// </summary>
    public void DropWeapon()
    {
        Owner = null;

        //be sure to reset attack vars
        ReleaseAttack();

        //call event
        onDropWeapon?.Invoke();

        //destroy weapon
        Destroy(gameObject);
    }

    #endregion

    #region abstracts

    public abstract void PressAttack();
    public abstract void ReleaseAttack();

    #endregion
}