using System.Collections;
using UnityEngine;
using redd096;
using UnityEngine.UI;

public class WeaponComponentBarsFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] WeaponComponent weaponComponent;

    [Header("Bars (already in scene/prefab)")]
    [SerializeField] Image rateOfFireImage = default;
    [SerializeField] Image reloadImage = default;
    [SerializeField] bool showRateOfFireAlsoOnAutomatic = false;

    Coroutine rateOfFireCoroutine;
    Coroutine reloadCoroutine;

    WeaponRange weaponRange;

    void OnEnable()
    {
        //get references
        if (weaponComponent == null) weaponComponent = GetComponentInParent<WeaponComponent>();

        //by default disable bars
        if (rateOfFireImage) rateOfFireImage.gameObject.SetActive(false);
        if (reloadImage) reloadImage.gameObject.SetActive(false);

        //add events
        if (weaponComponent)
        {
            weaponComponent.onPickWeapon += OnPickWeapon;
            weaponComponent.onDropWeapon += OnDropWeapon;

            //add events if has weapon
            OnPickWeapon();
        }
    }

    void OnDisable()
    {
        //remove events
        if (weaponComponent)
        {
            weaponComponent.onPickWeapon -= OnPickWeapon;
            weaponComponent.onDropWeapon -= OnDropWeapon;

            //remove events if has weapon
            OnDropWeapon();
        }
    }

    #region private API

    void OnPickWeapon()
    {
        //add events if has weapon
        if (weaponComponent && weaponComponent.CurrentWeapon && weaponComponent.CurrentWeapon is WeaponRange)
        {
            //save ref
            weaponRange = weaponComponent.CurrentWeapon as WeaponRange;

            weaponRange.onShoot += OnShoot;
            weaponRange.onStartReload += OnStartReload;
            weaponRange.onAbortReload += OnAbortReload;
        }
    }

    void OnDropWeapon()
    {
        //remove events if has weapon
        if (weaponRange)
        {
            weaponRange.onShoot -= OnShoot;
            weaponRange.onStartReload -= OnStartReload;
            weaponRange.onAbortReload -= OnAbortReload;
        }

        //be sure to stop coroutines
        if (rateOfFireCoroutine != null) StopCoroutine(rateOfFireCoroutine);
        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);

        //be sure to disable bars
        if (rateOfFireImage) rateOfFireImage.gameObject.SetActive(false);
        if (reloadImage) reloadImage.gameObject.SetActive(false);
    }

    void OnShoot()
    {
        //start rate of fire coroutine
        if (rateOfFireCoroutine != null)
            StopCoroutine(rateOfFireCoroutine);

        rateOfFireCoroutine = StartCoroutine(RateOfFireCoroutine());
    }

    void OnStartReload()
    {
        //start reload coroutine
        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);

        reloadCoroutine = StartCoroutine(ReloadCoroutine());
    }

    void OnAbortReload()
    {
        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);

        //disable bar
        if(reloadImage) reloadImage.gameObject.SetActive(false);
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

    IEnumerator ReloadCoroutine()
    {
        if (weaponRange && reloadImage)
        {
            //enable bar
            reloadImage.fillAmount = 1;
            reloadImage.gameObject.SetActive(true);

            //update bar
            float delta = 0;
            while (delta < 1)
            {
                delta += Time.deltaTime / weaponRange.delayReload;
                reloadImage.fillAmount = 1 - delta;

                yield return null;
            }

            //disable bar
            reloadImage.gameObject.SetActive(false);
        }
    }

    #endregion
}
