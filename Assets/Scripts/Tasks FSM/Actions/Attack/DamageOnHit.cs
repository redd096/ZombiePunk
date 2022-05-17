using System.Collections.Generic;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Attack/Damage On Hit")]
public class DamageOnHit : ActionTask
{
    [Header("Necessary Components - default get from parent")]
    [SerializeField] CollisionEventToChilds collisionEventToChilds = default;

    [Header("Damage")]
    [SerializeField] List<Character.ECharacterType> charactersToHit = new List<Character.ECharacterType>() { Character.ECharacterType.Player };
    [SerializeField] bool hitAlsoNotCharacters = true;
    [SerializeField] float damage = 10;
    [SerializeField] float pushForce = 10;
    [Tooltip("Necessary for on collision stay, to not call damage every frame")] [SerializeField] float delayBetweenAttacks = 1;

    [Header("DEBUG")]
    [SerializeField] bool disableBaseDamageCharacterOnHit = true;

    Redd096Main self;
    Character selfCharacter;
    DamageCharacterOnHit damageCharacterOnHit;
    Dictionary<Redd096Main, float> hits = new Dictionary<Redd096Main, float>();
    bool isActive;
    Redd096Main hitMain;

    void OnEnable()
    {
        //get references
        if (collisionEventToChilds == null)
            collisionEventToChilds = GetStateMachineComponent<CollisionEventToChilds>();

        //add events
        if (collisionEventToChilds)
        {
            collisionEventToChilds.onTriggerEnter2D += OnOwnerCollisionEnter2D;
            collisionEventToChilds.onTriggerStay2D += OnOwnerCollisionStay2D;
            collisionEventToChilds.onTriggerExit2D += OnOwnerCollisionExit2D;
        }
    }

    void OnDisable()
    {
        //remove events
        if (collisionEventToChilds)
        {
            collisionEventToChilds.onTriggerEnter2D -= OnOwnerCollisionEnter2D;
            collisionEventToChilds.onTriggerStay2D -= OnOwnerCollisionStay2D;
            collisionEventToChilds.onTriggerExit2D -= OnOwnerCollisionExit2D;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        self = GetStateMachineComponent<Redd096Main>();
        selfCharacter = self ? self as Character : null;
        damageCharacterOnHit = GetStateMachineComponent<DamageCharacterOnHit>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //enable only when the task is active
        isActive = true;

        //disable base damage character on hit
        if(disableBaseDamageCharacterOnHit && damageCharacterOnHit)
        {
            damageCharacterOnHit.SetCanDoDamage(false);
        }
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //disable when exit from the task
        isActive = false;

        //re-enable base damage character on hit
        if (disableBaseDamageCharacterOnHit && damageCharacterOnHit)
        {
            damageCharacterOnHit.SetCanDoDamage(true);
        }
    }

    void OnOwnerCollisionEnter2D(Collider2D collision)
    {
        //check if hit and is not already in the list
        hitMain = collision.transform.GetComponentInParent<Redd096Main>();
        if (hitMain && hits.ContainsKey(hitMain) == false)
        {
            //be sure to not hit self
            if (self == null || hitMain != self)
            {
                //check can damage
                Character hitCharacter = hitMain as Character;
                if ((hitCharacter != null && charactersToHit.Contains(hitCharacter.CharacterType))        //be sure is a type of character can hit
                    || (hitCharacter == null && hitAlsoNotCharacters))                                    //or is not a character and can hit also them
                {
                    //damage it and add to the list
                    OnHit(collision, hitMain);
                    hits.Add(hitMain, Time.time + delayBetweenAttacks);    //set timer
                }
            }
        }
    }

    void OnOwnerCollisionStay2D(Collider2D collision)
    {
        //check if hit and is in the list
        hitMain = collision.transform.GetComponentInParent<Redd096Main>();
        if (hitMain && hits.ContainsKey(hitMain))
        {
            //damage after delay
            if (Time.time > hits[hitMain])
            {
                OnHit(collision, hitMain);
                hits[hitMain] = Time.time + delayBetweenAttacks;       //reset timer
            }
        }
    }

    void OnOwnerCollisionExit2D(Collider2D collision)
    {
        //check if exit hit and is in the list
        hitMain = collision.transform.GetComponentInParent<Redd096Main>();
        if (hitMain && hits.ContainsKey(hitMain))
        {
            //remove from the list
            hits.Remove(hitMain);
        }
    }

    void OnHit(Collider2D collision, Redd096Main hit)
    {
        if (isActive == false)
            return;

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
            hit.GetSavedComponent<HealthComponent>().GetDamage(damage, selfCharacter, collision.ClosestPoint(transform.position));

        if (hit && hit.GetSavedComponent<MovementComponent>())
            hit.GetSavedComponent<MovementComponent>().PushInDirection(((Vector2)hit.transform.position - collision.ClosestPoint(transform.position)).normalized, pushForce);
    }
}
