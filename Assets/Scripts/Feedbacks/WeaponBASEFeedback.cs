using UnityEngine;
using Sirenix.OdinInspector;
using redd096;

public class WeaponBASEFeedback : MonoBehaviour
{
    [Header("Sprite to flip - default get in children")]
    [SerializeField] SpriteRenderer spriteToFlip = default;

    [Header("Pivot - default is this transform")]
    [SerializeField] Transform objectPivot = default;
    [SerializeField] float offsetFromPlayer = 1;

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
        if (spriteToFlip == null) spriteToFlip = GetComponentInChildren<SpriteRenderer>();
        if (objectPivot == null) objectPivot = transform;

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

    void Update()
    {
        //rotate weapon with aim and set position
        RotateWeapon();
        MoveWeapon();
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

    void RotateWeapon()
    {
        //rotate weapon with aim
        if (weaponBASE.Owner && weaponBASE.Owner.GetSavedComponent<AimComponent>())
        {
            Vector2 aimDirection = weaponBASE.Owner.GetSavedComponent<AimComponent>().AimDirectionInput;
            objectPivot.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * aimDirection);

            //when rotate to left, flip Y to not be upside down
            spriteToFlip.flipY = aimDirection.x < 0;
        }
    }

    void MoveWeapon()
    {
        if(weaponBASE.Owner && weaponBASE.Owner.GetSavedComponent<AimComponent>())
        {
            objectPivot.position = (Vector2)weaponBASE.Owner.transform.position + (weaponBASE.Owner.GetSavedComponent<AimComponent>().AimDirectionInput * offsetFromPlayer);
        }
    }

    #endregion
}
