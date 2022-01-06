using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/Attack/Damage On Hit")]
public class DamageOnHit : ActionTask
{
    [Header("Damage")]
    [SerializeField] List<Character.ECharacterType> charactersToHit = new List<Character.ECharacterType>() { Character.ECharacterType.AI };
    [SerializeField] bool hitAlsoNotCharacters = true;
    [SerializeField] float damage = 10;
    [SerializeField] float pushForce = 10;
    [Tooltip("Necessary for on collision stay, to not call damage every frame")] [SerializeField] float delayBetweenAttacks = 1;

    Redd096Main self;
    NotifyCollisions notifyCollisions;
    Dictionary<Redd096Main, float> hits = new Dictionary<Redd096Main, float>();

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        self = GetStateMachineComponent<Redd096Main>();
        notifyCollisions = GetStateMachineComponent<NotifyCollisions>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //enable only when the task is active
        if(notifyCollisions)
        {
            notifyCollisions.onCollisionEnter += OnOwnerCollisionEnter2D;
            notifyCollisions.onCollisionStay += OnOwnerCollisionStay2D;
            notifyCollisions.onCollisionExit += OnOwnerCollisionExit2D;
        }
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //disable when exit from the task
        if (notifyCollisions)
        {
            notifyCollisions.onCollisionEnter -= OnOwnerCollisionEnter2D;
            notifyCollisions.onCollisionStay -= OnOwnerCollisionStay2D;
            notifyCollisions.onCollisionExit -= OnOwnerCollisionExit2D;
        }
    }

    void OnOwnerCollisionEnter2D(Collision2D collision)
    {
        //check if hit and is not already in the list
        Redd096Main hit = collision.gameObject.GetComponentInParent<Redd096Main>();
        if (hit && hits.ContainsKey(hit) == false)
        {
            //be sure to not hit self
            if (self == null || hit != self)
            {
                //check can damage
                Character hitCharacter = hit as Character;
                if ((hitCharacter != null && charactersToHit.Contains(hitCharacter.CharacterType))        //be sure is a type of character can hit
                    || (hitCharacter == null && hitAlsoNotCharacters))                                    //or is not a character and can hit also them
                {
                    //damage it and add to the list
                    OnHit(collision, hit);
                    hits.Add(hit, Time.time + delayBetweenAttacks);    //set timer
                }
            }
        }
    }

    void OnOwnerCollisionStay2D(Collision2D collision)
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

    void OnOwnerCollisionExit2D(Collision2D collision)
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

        //do damage and push back
        if (hit.GetSavedComponent<HealthComponent>())
            hit.GetSavedComponent<HealthComponent>().GetDamage(damage);

        if (hit && hit.GetSavedComponent<MovementComponent>())
            hit.GetSavedComponent<MovementComponent>().PushInDirection(((Vector2)hit.transform.position - collision.GetContact(0).point).normalized, pushForce);
    }
}
