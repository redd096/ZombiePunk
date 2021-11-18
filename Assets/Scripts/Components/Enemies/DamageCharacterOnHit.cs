using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCharacterOnHit : MonoBehaviour
{
    [Header("Use trigger or collision enter")]
    [SerializeField] bool useTrigger = true;

    [Header("Damage")]
    [SerializeField] bool friendlyFire = false;
    [SerializeField] float damage = 10;
    [SerializeField] float pushForce = 10;

    Character character;

    //events
    public System.Action onHit { get; set; }

    void Awake()
    {
        character = GetComponent<Character>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //be sure which collider use, then call OnHit
        if (useTrigger)
            OnHit(collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //be sure which collider use, then call OnHit
        if (useTrigger == false)
            OnHit(collision.gameObject);
    }

    void OnHit(GameObject hit)
    {
        //check if hit character, and if is enabled friendly fire or is another type of character
        Character hitCharacter = hit.GetComponentInParent<Character>();
        if (hitCharacter && (character == null || friendlyFire || hitCharacter.CharacterType != character.CharacterType))
        {
            onHit?.Invoke();

            //if hit something, do damage and push back
            if (hitCharacter.GetSavedComponent<HealthComponent>()) 
                hitCharacter.GetSavedComponent<HealthComponent>().GetDamage(damage);

            if (hitCharacter && hitCharacter.GetSavedComponent<MovementComponent>()) 
                hitCharacter.GetSavedComponent<MovementComponent>().PushInDirection((hit.transform.position - transform.position).normalized, pushForce);
        }
    }
}
