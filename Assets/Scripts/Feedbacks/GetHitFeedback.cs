﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//using NaughtyAttributes;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Get Hit Feedback")]
    public class GetHitFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] HealthComponent healthComponent;

        [Header("Blink - Default get component in children")]
        [SerializeField] bool blinkOnGetDamage = true;
        [EnableIf("blinkOnGetDamage")] [SerializeField] SpriteRenderer[] spritesToUse = default;
        [EnableIf("blinkOnGetDamage")] [SerializeField] Material blinkMaterial = default;
        [EnableIf("blinkOnGetDamage")] [SerializeField] float blinkDuration = 0.2f;

        [Header("On Get Damage")]
        [SerializeField] InstantiatedGameObjectStruct gameObjectOnGetDamage = default;
        [SerializeField] ParticleSystem particlesOnGetDamage = default;
        [SerializeField] AudioClass audioOnGetDamage = default;

        [Header("On Get Damage Gamepad Vibration")]
        [SerializeField] bool gamepadVibration = false;
        [EnableIf("gamepadVibration")] [SerializeField] bool customVibration = false;
        [EnableIf("gamepadVibration", "customVibration")] [SerializeField] float vibrationDuration = 0.1f;
        [EnableIf("gamepadVibration", "customVibration")] [SerializeField] float lowFrequency = 0.5f;
        [EnableIf("gamepadVibration", "customVibration")] [SerializeField] float highFrequency = 0.8f;

        [Header("On Die (instantiate every element in array)")]
        [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnDie = default;
        [SerializeField] ParticleSystem[] particlesOnDie = default;
        [SerializeField] AudioClass[] audiosOnDie = default;

        Character selfCharacter;
        Dictionary<SpriteRenderer, Material> savedMaterials = new Dictionary<SpriteRenderer, Material>();
        Coroutine blinkCoroutine;

        void Awake()
        {
            //save materials
            if (spritesToUse == null || spritesToUse.Length <= 0) spritesToUse = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sprite in spritesToUse)
                savedMaterials.Add(sprite, sprite.material);
        }

        void OnEnable()
        {
            //get references
            if (healthComponent == null) healthComponent = GetComponentInParent<HealthComponent>();
            if (spritesToUse == null || spritesToUse.Length <= 0) spritesToUse = GetComponentsInChildren<SpriteRenderer>();
            if (selfCharacter == null) selfCharacter = GetComponentInParent<Character>();

            //add events
            if (healthComponent)
            {
                healthComponent.onGetDamage += OnGetDamage;
                healthComponent.onDie += OnDie;
            }
        }

        void OnDisable()
        {
            //remove events
            if (healthComponent)
            {
                healthComponent.onGetDamage -= OnGetDamage;
                healthComponent.onDie -= OnDie;
            }
        }

        #region private API

        void OnGetDamage()
        {
            //instantiate vfx and sfx
            InstantiateGameObjectManager.instance.Play(gameObjectOnGetDamage, transform.position, transform.rotation);
            ParticlesManager.instance.Play(particlesOnGetDamage, transform.position, transform.rotation);
            SoundManager.instance.Play(audioOnGetDamage, transform.position);

            //blink
            if (blinkCoroutine == null && blinkOnGetDamage)
                blinkCoroutine = StartCoroutine(BlinkCoroutine());

            //gamepad vibration
            if (selfCharacter && selfCharacter.CharacterType == Character.ECharacterType.Player)
            {
                if (gamepadVibration && GamepadVibration.instance)
                {
                    //custom or default
                    if (customVibration)
                        GamepadVibration.instance.StartVibration(vibrationDuration, lowFrequency, highFrequency);
                    else
                        GamepadVibration.instance.StartVibration();
                }
            }

            //spawn blood on screen
            if(selfCharacter && selfCharacter.CharacterType == Character.ECharacterType.Player)
            {
                if (GameManager.instance && GameManager.instance.uiManager)
                    GameManager.instance.uiManager.ShowBloodOnScreen();
            }
        }

        IEnumerator BlinkCoroutine()
        {
            //set blink material, wait, then back to saved material
            foreach (SpriteRenderer sprite in savedMaterials.Keys)
                sprite.material = blinkMaterial;

            yield return new WaitForSeconds(blinkDuration);

            foreach (SpriteRenderer sprite in savedMaterials.Keys)
                sprite.material = savedMaterials[sprite];

            blinkCoroutine = null;
        }

        void OnDie(HealthComponent whoDied)
        {
            //instantiate vfx and sfx
            foreach (InstantiatedGameObjectStruct go in gameObjectsOnDie)
                InstantiateGameObjectManager.instance.Play(go, transform.position, transform.rotation);         //instantiate every element in array

            foreach (ParticleSystem particle in particlesOnDie)
                ParticlesManager.instance.Play(particle, transform.position, transform.rotation);               //instantiate every element in array

            foreach (AudioClass audio in audiosOnDie)
                SoundManager.instance.Play(audio, transform.position);                                          //instantiate every element in array
        }

        #endregion
    }
}