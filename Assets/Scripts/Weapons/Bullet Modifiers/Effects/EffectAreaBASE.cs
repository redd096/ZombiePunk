using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;

public abstract class EffectAreaBASE : EffectBASE
{
    protected abstract float delayOnTriggerStay { get; }

    //events
    public System.Action onHit { get; set; }

    protected Dictionary<Redd096Main, float> hits = new Dictionary<Redd096Main, float>();
    Redd096Main hitMain;

    protected virtual void OnDisable()
    {
        //clear vars
        hits.Clear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //check if hit and is not already in the list
        hitMain = collision.transform.GetComponentInParent<Redd096Main>();
        if (hitMain && hits.ContainsKey(hitMain) == false)
        {
            //be sure to not hit owner (or is enabled can hit also owner)
            if (owner == null || hitMain != owner || hitAlsoWhoCreatedThis)
            {
                //check can damage
                Character hitCharacter = hitMain as Character;
                if ((hitCharacter != null && charactersToHit.Contains(hitCharacter.CharacterType))        //be sure is a type of character can hit
                    || (hitCharacter == null && hitAlsoNotCharacters))                                    //or is not a character and can hit also them
                {
                    //check same type of enemy
                    if (hitAlsoEnemiesOfSameType == false                                                                                                                //if can't hit same type
                        && hitCharacter && owner && hitCharacter.CharacterType == Character.ECharacterType.AI && owner.CharacterType == Character.ECharacterType.AI     //and are both enemies
                        && hitCharacter.EnemyType == owner.EnemyType)                                                                                                   //of same type
                    {
                        return;                                                                                                                                         //then return
                    }

                    //damage it and add to the list
                    OnHit(collision, hitMain);
                    hits.Add(hitMain, Time.time + delayOnTriggerStay);    //set timer
                }
            }
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        //check if hit and is in the list
        hitMain = collision.transform.GetComponentInParent<Redd096Main>();
        if (hitMain && hits.ContainsKey(hitMain))
        {
            //damage after delay
            if (Time.time > hits[hitMain])
            {
                OnHit(collision, hitMain);
                hits[hitMain] = Time.time + delayOnTriggerStay;       //reset timer
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        //check if exit hit and is in the list
        hitMain = collision.transform.GetComponentInParent<Redd096Main>();
        if (hitMain && hits.ContainsKey(hitMain))
        {
            //remove from the list
            hits.Remove(hitMain);
        }
    }

    protected virtual void OnHit(Collider2D collision, Redd096Main hit)
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
    }
}
