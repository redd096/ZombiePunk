using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Weapons/Bullet Modifiers/Effects/Acid Effect")]
[RequireComponent(typeof(Rigidbody2D))]
public class AcidEffect : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] List<Character.ECharacterType> charactersToHit = new List<Character.ECharacterType>() { Character.ECharacterType.Player };
    [SerializeField] bool hitAlsoNotCharacters = true;
    [SerializeField] bool hitAlsoWhoCreatedAcid = false;        //can hit owner
    [SerializeField] bool hitAlsoEnemiesOfSameType = false;     //can hit same type of enemy

    [Header("Timer Disappear (0 = no disappear)")]
    [SerializeField] float duration = 2;

    [Header("Damage")]
    [SerializeField] float damage = 10;
    [SerializeField] float pushForce = 10;
    [Tooltip("Necessary for on collision stay, to not call damage every frame")] [SerializeField] float delayBetweenAttacks = 1;

    //events
    public System.Action onHit { get; set; }

    Character owner;
    Dictionary<Redd096Main, float> hits = new Dictionary<Redd096Main, float>();

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="owner"></param>
    public void Init(Character owner)
    {
        this.owner = owner;

        //disappear coroutine
        if (duration > 0)
        {
            StartCoroutine(DisappearCoroutine());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //check if hit and is not already in the list
        Redd096Main hit = collision.gameObject.GetComponentInParent<Redd096Main>();
        if (hit && hits.ContainsKey(hit) == false)
        {
            //be sure to not hit owner (or is enabled can hit also owner)
            if (owner == null || hit != owner || hitAlsoWhoCreatedAcid)
            {
                //check can damage
                Character hitCharacter = hit as Character;
                if ((hitCharacter != null && charactersToHit.Contains(hitCharacter.CharacterType))        //be sure is a type of character can hit
                    || (hitCharacter == null && hitAlsoNotCharacters))                                    //or is not a character and can hit also them
                {
                    //check same type of enemy
                    if(hitAlsoEnemiesOfSameType == false                                                                                                                //if can't hit same type
                        && hitCharacter && owner && hitCharacter.CharacterType == Character.ECharacterType.AI && owner.CharacterType == Character.ECharacterType.AI     //and are both enemies
                        && hitCharacter.EnemyType == owner.EnemyType)                                                                                                   //of same type
                    {
                        return;                                                                                                                                         //then return
                    }

                    //damage it and add to the list
                    OnHit(collision, hit);
                    hits.Add(hit, Time.time + delayBetweenAttacks);    //set timer
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        //check if hit and is in the list
        Redd096Main hit = collision.gameObject.GetComponentInParent<Redd096Main>();
        if (hit && hits.ContainsKey(hit))
        {
            //damage after delay
            if (Time.time > hits[hit])
            {
                OnHit(collision, hit);
                hits[hit] = Time.time + delayBetweenAttacks;       //reset timer
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        //check if exit hit and is in the list
        Redd096Main hit = collision.gameObject.GetComponentInParent<Redd096Main>();
        if (hit && hits.ContainsKey(hit))
        {
            //remove from the list
            hits.Remove(hit);
        }
    }

    void OnHit(Collision2D collision, Redd096Main hit)
    {
        //if there is no hit, do nothing
        if (hit == null)
        {
            //and remove from the list
            if (hits.ContainsKey(hit))
                hits.Remove(hit);

            return;
        }

        //call event
        onHit?.Invoke();

        //do damage and push back
        if (hit.GetSavedComponent<HealthComponent>())
            hit.GetSavedComponent<HealthComponent>().GetDamage(damage, owner, collision.GetContact(0).point);

        if (hit && hit.GetSavedComponent<MovementComponent>())
            hit.GetSavedComponent<MovementComponent>().PushInDirection(((Vector2)hit.transform.position - collision.GetContact(0).point).normalized, pushForce);
    }

    IEnumerator DisappearCoroutine()
    {
        //wait
        yield return new WaitForSeconds(duration);

        //and destroy
        Pooling.Destroy(gameObject);
    }
}
