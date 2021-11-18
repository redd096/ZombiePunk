using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using redd096;

public class GetHitFeedback : MonoBehaviour
{
    [Header("Blink - Default get component in children")]
    [SerializeField] SpriteRenderer[] spritesToUse = default;
    [SerializeField] Material blinkMaterial = default;
    [SerializeField] float blinkDuration = 0.2f;

    [Header("On Get Damage")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnGetDamage = default;
    [SerializeField] ParticleSystem particlesOnGetDamage = default;
    [SerializeField] AudioClass audioOnGetDamage = default;

    [Header("On Die")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnDie = default;
    [SerializeField] ParticleSystem particlesOnDie = default;
    [SerializeField] AudioClass audioOnDie = default;

    HealthComponent component;
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
        component = GetComponent<HealthComponent>();
        if (spritesToUse == null || spritesToUse.Length <= 0) spritesToUse = GetComponentsInChildren<SpriteRenderer>();

        //add events
        if (component)
        {
            component.onGetDamage += OnGetDamage;
            component.onDie += OnDie;
        }
    }

    void OnDisable()
    {
        //remove events
        if (component)
        {
            component.onGetDamage -= OnGetDamage;
            component.onDie -= OnDie;
        }
    }

    #region private API

    void OnGetDamage()
    {
        //instantiate vfx and sfx
        foreach (InstantiatedGameObjectStruct objectOnGetDamage in gameObjectsOnGetDamage)
        {
            InstantiateGameObjectManager.instance.Play(objectOnGetDamage, transform.position, transform.rotation);
        }
        ParticlesManager.instance.Play(particlesOnGetDamage, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnGetDamage, transform.position);

        //blink
        if (blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkCoroutine());
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

    void OnDie()
    {
        //instantiate vfx and sfx
        foreach (InstantiatedGameObjectStruct objectOnDie in gameObjectsOnDie)
        {
            InstantiateGameObjectManager.instance.Play(objectOnDie, transform.position, transform.rotation);
        }
        ParticlesManager.instance.Play(particlesOnDie, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnDie, transform.position);
    }

    #endregion
}
