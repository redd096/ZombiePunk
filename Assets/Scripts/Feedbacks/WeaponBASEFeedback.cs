using UnityEngine;
using Sirenix.OdinInspector;
using redd096;

public class WeaponBASEFeedback : MonoBehaviour
{
    [Header("On Pick")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnPick = default;
    [SerializeField] ParticleSystem particlesOnPick = default;
    [SerializeField] AudioClass audioOnPick = default;

    [Header("On Pick Camera Shake")]
    [SerializeField] bool cameraShakeOnPick = true;
    [EnableIf("cameraShakeOnPick")] [SerializeField] bool customShakeOnPick = false;
    [EnableIf("@cameraShakeOnPick && customShakeOnPick")] [SerializeField] float shakeDurationOnPick = 1;
    [EnableIf("@cameraShakeOnPick && customShakeOnPick")] [SerializeField] float shakeAmountOnPick = 0.7f;

    [Header("On Drop")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnDrop = default;
    [SerializeField] ParticleSystem particlesOnDrop = default;
    [SerializeField] AudioClass audioOnDrop = default;

    [Header("On Drop Camera Shake")]
    [SerializeField] bool cameraShakeOnDrop = true;
    [EnableIf("cameraShakeOnDrop")] [SerializeField] bool customShakeOnDrop = false;
    [EnableIf("@cameraShakeOnDrop && customShakeOnDrop")] [SerializeField] float shakeDurationOnDrop = 1;
    [EnableIf("@cameraShakeOnDrop && customShakeOnDrop")] [SerializeField] float shakeAmountOnDrop = 0.7f;

    WeaponBASE weaponBASE;

    void OnEnable()
    {
        //get references
        weaponBASE = GetComponent<WeaponBASE>();

        //add events
        if (weaponBASE)
        {
            weaponBASE.onPickWeapon += OnPickWeapon;
            weaponBASE.onDropWeapon += OnDropWeapon;
        }
    }

    void OnDisable()
    {
        //remove events
        if (weaponBASE)
        {
            weaponBASE.onPickWeapon -= OnPickWeapon;
            weaponBASE.onDropWeapon -= OnDropWeapon;
        }
    }

    #region private API

    void OnPickWeapon()
    {
        //instantiate vfx and sfx
        InstantiateGameObjectManager.instance.Play(gameObjectOnPick, transform.position, transform.rotation);
        ParticlesManager.instance.Play(particlesOnPick, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnPick, transform.position);

        //camera shake
        if (cameraShakeOnPick && CameraShake.instance)
        {
            //custom or default
            if (customShakeOnPick)
                CameraShake.instance.StartShake(shakeDurationOnPick, shakeAmountOnPick);
            else
                CameraShake.instance.StartShake();
        }
    }

    void OnDropWeapon()
    {
        //instantiate vfx and sfx
        InstantiateGameObjectManager.instance.Play(gameObjectOnDrop, transform.position, transform.rotation);
        ParticlesManager.instance.Play(particlesOnDrop, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnDrop, transform.position);

        //camera shake
        if (cameraShakeOnDrop && CameraShake.instance)
        {
            //custom or default
            if (customShakeOnDrop)
                CameraShake.instance.StartShake(shakeDurationOnDrop, shakeAmountOnDrop);
            else
                CameraShake.instance.StartShake();
        }
    }

    #endregion
}
