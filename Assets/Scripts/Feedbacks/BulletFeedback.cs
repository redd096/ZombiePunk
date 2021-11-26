﻿using UnityEngine;
using Sirenix.OdinInspector;
using redd096;

public class BulletFeedback : MonoBehaviour
{
    [Header("On Hit (also if move through this object)")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnHit = default;
    [SerializeField] ParticleSystem particlesOnHit = default;
    [SerializeField] AudioClass audioOnHit = default;

    [Header("On Hit something that destroy bullet")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnLastHit = default;
    [SerializeField] ParticleSystem particlesOnLastHit = default;
    [SerializeField] AudioClass audioOnLastHit = default;

    [Header("On AutoDestruction")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnAutodestruction = default;
    [SerializeField] ParticleSystem particlesOnAutodestruction = default;
    [SerializeField] AudioClass audioOnAutodestruction = default;

    [Header("On Destroy (both autodestruction or hit)")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnDestroy = default;
    [SerializeField] ParticleSystem particlesOnDestroy = default;
    [SerializeField] AudioClass audioOnDestroy = default;

    [Header("Camera Shake")]
    [SerializeField] bool cameraShakeOnHit = false;
    [SerializeField] bool cameraShakeOnHitSomethingThatDestroyBullet = false;
    [SerializeField] bool cameraShakeOnAutoDestruction = false;
    [SerializeField] bool cameraShakeOnDestroy = false;
    [EnableIf("@cameraShakeOnHit || cameraShakeOnHitSomethingThatDestroyBullet || cameraShakeOnAutoDestruction || cameraShakeOnDestroy")] [SerializeField] bool customShake = false;
    [EnableIf("@cameraShakeOnHit || cameraShakeOnHitSomethingThatDestroyBullet || cameraShakeOnAutoDestruction || cameraShakeOnDestroy")] [SerializeField] float shakeDuration = 1;
    [EnableIf("@cameraShakeOnHit || cameraShakeOnHitSomethingThatDestroyBullet || cameraShakeOnAutoDestruction || cameraShakeOnDestroy")] [SerializeField] float shakeAmount = 0.7f;

    Bullet bullet;

    void OnEnable()
    {
        //get references
        bullet = GetComponent<Bullet>();

        //add events
        if (bullet)
        {
            bullet.onHit += OnHit;
            bullet.onLastHit += OnLastHit;
            bullet.onAutodestruction += OnAutodestruction;
            bullet.onDie += OnDie;
        }
    }

    void OnDisable()
    {
        //remove events
        if (bullet)
        {
            bullet.onHit -= OnHit;
            bullet.onLastHit -= OnLastHit;
            bullet.onAutodestruction -= OnAutodestruction;
            bullet.onDie -= OnDie;
        }
    }

    void OnHit(GameObject hit)
    {
        //instantiate vfx and sfx
        InstantiateGameObjectManager.instance.Play(gameObjectOnHit, transform.position, transform.rotation);
        ParticlesManager.instance.Play(particlesOnHit, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnHit, transform.position);

        //camera shake
        if (cameraShakeOnHit)
        {
            //custom or default
            if (customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }

    void OnLastHit(GameObject hit)
    {
        //instantiate vfx and sfx
        InstantiateGameObjectManager.instance.Play(gameObjectOnLastHit, transform.position, transform.rotation);
        ParticlesManager.instance.Play(particlesOnLastHit, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnLastHit, transform.position);

        //camera shake
        if (cameraShakeOnHitSomethingThatDestroyBullet)
        {
            //custom or default
            if (customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }

    void OnAutodestruction()
    {
        //instantiate vfx and sfx
        InstantiateGameObjectManager.instance.Play(gameObjectOnAutodestruction, transform.position, transform.rotation);
        ParticlesManager.instance.Play(particlesOnAutodestruction, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnAutodestruction, transform.position);

        //camera shake
        if (cameraShakeOnAutoDestruction)
        {
            //custom or default
            if (customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }

    void OnDie()
    {
        //instantiate vfx and sfx
        InstantiateGameObjectManager.instance.Play(gameObjectOnDestroy, transform.position, transform.rotation);
        ParticlesManager.instance.Play(particlesOnDestroy, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnDestroy, transform.position);

        //camera shake
        if (cameraShakeOnDestroy)
        {
            //custom or default
            if (customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }
}