using System.Collections;
using UnityEngine;
using redd096;

public class GetHitFeedback : MonoBehaviour
{
    [Header("Blink - Default get component in children")]
    [SerializeField] SpriteRenderer spriteToUse = default;
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
    Material savedMaterial;
    Coroutine blinkCoroutine;

    void Awake()
    {
        //save material
        if (spriteToUse == null) spriteToUse = GetComponentInChildren<SpriteRenderer>();
        if (spriteToUse) savedMaterial = spriteToUse.material;
    }

    void OnEnable()
    {
        //get references
        component = GetComponent<HealthComponent>();
        if (spriteToUse == null) spriteToUse = GetComponentInChildren<SpriteRenderer>();

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
        if (spriteToUse) 
            spriteToUse.material = blinkMaterial;

        yield return new WaitForSeconds(blinkDuration);

        if (spriteToUse) 
            spriteToUse.material = savedMaterial;
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
}
