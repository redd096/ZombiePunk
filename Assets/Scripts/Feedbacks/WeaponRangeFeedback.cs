using UnityEngine;
using Sirenix.OdinInspector;
using redd096;

public class WeaponRangeFeedback : MonoBehaviour
{
    [Header("On Instantiate Bullet")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnInstantiateBullet = default;
    [SerializeField] ParticleSystem particlesOnInstantiateBullet = default;
    [SerializeField] AudioClass audioOnInstantiateBullet = default;

    [Header("On Shoot - main barrel by default is transform")]
    [SerializeField] Transform mainBarrel = default;
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnShoot = default;
    [SerializeField] ParticleSystem particlesOnShoot = default;
    [SerializeField] AudioClass audioOnShoot = default;

    [Header("On Shoot Camera Shake")]
    [SerializeField] bool cameraShake = true;
    [EnableIf("cameraShake")] [SerializeField] bool customShake = false;
    [EnableIf("@cameraShake && customShake")] [SerializeField] float shakeDuration = 1;
    [EnableIf("@cameraShake && customShake")] [SerializeField] float shakeAmount = 0.7f;

    [Header("On Press Attack - barrel by default is transform (need audio source)")]
    [SerializeField] Transform barrelOnPress = default;
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnPress = default;
    [SerializeField] ParticleSystem particlesOnPress = default;
    [SerializeField] AudioSource audioSourcePrefab = default;
    [SerializeField] AudioClass audioOnPress = default;

    //on release attack
    GameObject instantiatedGameObjectOnPress;
    ParticleSystem instantiatedParticlesOnPress;
    AudioSource instantiatedAudioOnPress;

    WeaponRange weaponRange;

    void OnEnable()
    {
        //get references
        weaponRange = GetComponent<WeaponRange>();
        if (mainBarrel == null) mainBarrel = transform;
        if (barrelOnPress == null) barrelOnPress = transform;

        //add events
        if (weaponRange)
        {
            weaponRange.onInstantiateBullet += OnInstantiateBullet;
            weaponRange.onShoot += OnShoot;
            weaponRange.onPressAttack += OnPressAttack;
            weaponRange.onReleaseAttack += OnReleaseAttack;
        }
    }

    void OnDisable()
    {
        //remove events
        if (weaponRange)
        {
            weaponRange.onInstantiateBullet -= OnInstantiateBullet;
            weaponRange.onShoot -= OnShoot;
            weaponRange.onPressAttack -= OnPressAttack;
            weaponRange.onReleaseAttack -= OnReleaseAttack;
        }
    }

    #region private API

    void OnInstantiateBullet(Transform barrel)
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnInstantiateBullet, barrel.position, barrel.rotation);
        if (instantiatedGameObject)
        {
            //set parent
            instantiatedGameObject.transform.SetParent(transform);
        }
        ParticlesManager.instance.Play(particlesOnInstantiateBullet, barrel.position, barrel.rotation);
        SoundManager.instance.Play(audioOnInstantiateBullet, barrel.position);
    }

    void OnShoot()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnShoot, mainBarrel.position, mainBarrel.rotation);
        if (instantiatedGameObject)
        {
            //set parent
            instantiatedGameObject.transform.SetParent(transform);
        }
        ParticlesManager.instance.Play(particlesOnShoot, mainBarrel.position, mainBarrel.rotation);
        SoundManager.instance.Play(audioOnShoot, mainBarrel.position);

        //camera shake
        if (cameraShake && CameraShake.instance)
        {
            //custom or default
            if (customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }

    void OnPressAttack()
    {
        //if first time, instantiate prefabs
        {
            //instantiate game object
            if (gameObjectOnPress.instantiatedGameObject && instantiatedGameObjectOnPress == null)
            {
                instantiatedGameObjectOnPress = Instantiate(gameObjectOnPress.instantiatedGameObject);
            }
            //instantiate particles
            if (particlesOnPress && instantiatedParticlesOnPress == null)
            {
                instantiatedParticlesOnPress = Instantiate(particlesOnPress);
            }
            //instantiate audiosource
            if (audioOnPress.audioClip && audioSourcePrefab && instantiatedAudioOnPress == null)
            {
                instantiatedAudioOnPress = Instantiate(audioSourcePrefab);
                instantiatedAudioOnPress.clip = audioOnPress.audioClip;
                instantiatedAudioOnPress.volume = audioOnPress.volume;
                instantiatedAudioOnPress.spatialBlend = audioOnPress.is3D ? 1 : 0;
            }
        }

        //set game object
        if (instantiatedGameObjectOnPress)
        {
            instantiatedGameObjectOnPress.transform.position = barrelOnPress.position;
            instantiatedGameObjectOnPress.transform.rotation = barrelOnPress.rotation;

            //set parent
            instantiatedGameObjectOnPress.transform.SetParent(transform);

            instantiatedGameObjectOnPress.SetActive(true);
        }
        //set particles
        if (instantiatedParticlesOnPress)
        {
            instantiatedParticlesOnPress.transform.position = barrelOnPress.position;
            instantiatedParticlesOnPress.transform.rotation = barrelOnPress.rotation;

            //play
            instantiatedParticlesOnPress.gameObject.SetActive(true);
            instantiatedParticlesOnPress.Play();
        }
        //set audiosource
        if (instantiatedAudioOnPress)
        {
            instantiatedAudioOnPress.transform.position = barrelOnPress.position;

            //play
            instantiatedAudioOnPress.gameObject.SetActive(true);
            instantiatedAudioOnPress.Play();
        }
    }

    void OnReleaseAttack()
    {
        //deactive game object
        if (instantiatedGameObjectOnPress)
        {
            instantiatedGameObjectOnPress.SetActive(false);
        }
        //deactive particles
        if (instantiatedParticlesOnPress)
        {
            instantiatedParticlesOnPress.gameObject.SetActive(false);
        }
        //deactive sound
        if (instantiatedAudioOnPress)
        {
            instantiatedAudioOnPress.gameObject.SetActive(false);
        }
    }

    #endregion
}
