using System.Collections;
using UnityEngine;

public class GetHitFeedback : MonoBehaviour
{
    [Header("Default get component in children")]
    [SerializeField] SpriteRenderer spriteToFlip = default;
    [SerializeField] Material blinkMaterial = default;
    [SerializeField] float blinkDuration = 0.2f;

    HealthComponent component;
    Material savedMaterial;
    Coroutine blinkCoroutine;

    void Awake()
    {
        //get references
        component = GetComponent<HealthComponent>();
        if (spriteToFlip == null) spriteToFlip = GetComponentInChildren<SpriteRenderer>();
        if (spriteToFlip) savedMaterial = spriteToFlip.material;
    }

    void OnEnable()
    {
        //add events
        if (component)
            component.onGetDamage += OnGetDamage;
    }

    void OnDisable()
    {
        //remove events
        if (component)
            component.onGetDamage -= OnGetDamage;
    }

    void OnGetDamage()
    {
        //blink
        if (blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine()
    {
        //set blink material, wait, then back to saved material
        if (spriteToFlip) 
            spriteToFlip.material = blinkMaterial;

        yield return new WaitForSeconds(blinkDuration);

        if (spriteToFlip) 
            spriteToFlip.material = savedMaterial;
    }
}
