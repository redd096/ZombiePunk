using System.Collections;
using UnityEngine;
using redd096.GameTopDown2D;
using UnityEngine.UI;

[AddComponentMenu("redd096/Feedbacks/Weapon Bars Feedback")]
public class WeaponBarsFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] WeaponComponent weaponComponent;

    [Header("Bars (already in scene/prefab)")]
    [SerializeField] Image rateOfFireImage = default;
    [SerializeField] bool showRateOfFireAlsoOnAutomatic = false;

    Coroutine rateOfFireCoroutine;
    WeaponRange weaponRange;

    void OnEnable()
    {
        //get references
        if (weaponComponent == null) weaponComponent = GetComponentInParent<WeaponComponent>();

        //by default disable bars
        if (rateOfFireImage) rateOfFireImage.gameObject.SetActive(false);

        //add events
        if (weaponComponent)
        {
            weaponComponent.onChangeWeapon += OnChangeWeapon;

            //add events if has weapon
            OnChangeWeapon();
        }
    }

    void OnDisable()
    {
        //remove events
        if (weaponComponent)
        {
            weaponComponent.onChangeWeapon -= OnChangeWeapon;

            //remove events if has weapon
            DropWeapon();
        }
    }

    #region private API

    void OnChangeWeapon()
    {
        //add events if has weapon
        if (weaponComponent && weaponComponent.CurrentWeapon && weaponComponent.CurrentWeapon is WeaponRange)
        {
            //be sure to remove old weapon
            DropWeapon();

            //save ref
            weaponRange = weaponComponent.CurrentWeapon as WeaponRange;

            weaponRange.onShoot += OnShoot;
        }
    }

    void DropWeapon()
    {
        //remove events if has weapon
        if (weaponRange)
        {
            weaponRange.onShoot -= OnShoot;
        }

        //be sure to stop coroutines
        if (rateOfFireCoroutine != null) StopCoroutine(rateOfFireCoroutine);

        //be sure to disable bars
        if (rateOfFireImage) rateOfFireImage.gameObject.SetActive(false);
    }

    void OnShoot()
    {
        //start rate of fire coroutine
        if (rateOfFireCoroutine != null)
            StopCoroutine(rateOfFireCoroutine);

        rateOfFireCoroutine = StartCoroutine(RateOfFireCoroutine());
    }

    IEnumerator RateOfFireCoroutine()
    {
        if(weaponRange && rateOfFireImage)
        {
            //only if show also on automatic, or if is not automatic
            if (showRateOfFireAlsoOnAutomatic || weaponRange.Automatic == false)
            {
                //enable bar
                rateOfFireImage.fillAmount = 1;
                rateOfFireImage.gameObject.SetActive(true);

                //update bar
                float delta = 0;
                while (delta < 1)
                {
                    delta += Time.deltaTime / weaponRange.RateOfFire;
                    rateOfFireImage.fillAmount = 1 - delta;

                    yield return null;
                }

                //disable bar
                rateOfFireImage.gameObject.SetActive(false);
            }
        }
    }

    #endregion
}
