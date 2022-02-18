﻿using System.Collections;
using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/Weapons/Ammo")]
    public class Ammo : MonoBehaviour
    {
        [Header("Necessary Components - default get from this gameObject")]
        [SerializeField] CollisionComponent collisionComponent = default;

        [Header("Ammo")]
        [SerializeField] string ammoType = "GunAmmo";
        [SerializeField] int quantity = 1;
        [Tooltip("Can pick when full of this type of ammo? If true, this object will be destroyed, but no ammo will be added")] [SerializeField] bool canPickAlsoIfFull = false;

        [Header("Destroy when instantiated - 0 = no destroy")]
        [SerializeField] float timeBeforeDestroy = 0;

        public string AmmoType => ammoType;

        //events
        public System.Action<Ammo> onPick { get; set; }
        public System.Action<Ammo> onFailPick { get; set; }

        Character whoHit;
        AdvancedWeaponComponent whoHitComponent;
        bool alreadyUsed;

        void OnEnable()
        {
            //reset vars
            alreadyUsed = false;

            //if there is, start auto destruction timer
            if (timeBeforeDestroy > 0)
                StartCoroutine(AutoDestruction());

            //get references
            if (collisionComponent == null) 
                collisionComponent = GetComponent<CollisionComponent>();

            //warnings
            if (collisionComponent == null)
                Debug.LogWarning("Miss CollisionComponent on " + name);

            //add events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter += OnRDCollisionEvent;
                collisionComponent.onTriggerEnter += OnRDCollisionEvent;
            }
        }

        void OnDisable()
        {
            //remove events
            if (collisionComponent)
            {
                collisionComponent.onCollisionEnter -= OnRDCollisionEvent;
                collisionComponent.onTriggerEnter -= OnRDCollisionEvent;
            }
        }

        void OnRDCollisionEvent(RaycastHit2D collision)
        {
            if (alreadyUsed)
                return;

            //if hitted by player
            whoHit = collision.transform.GetComponentInParent<Character>();
            if (whoHit && whoHit.CharacterType == Character.ECharacterType.Player)
            {
                //and player has weapon component
                whoHitComponent = whoHit.GetSavedComponent<AdvancedWeaponComponent>();
                if (whoHitComponent)
                {
                    //if full of ammo, can't pick, call fail event
                    if (whoHitComponent.IsFullOfAmmo(ammoType) && canPickAlsoIfFull == false)
                    {
                        onFailPick?.Invoke(this);
                    }
                    //else, pick and add quantity
                    else
                    {
                        whoHitComponent.AddAmmo(ammoType, quantity);

                        //call event
                        onPick?.Invoke(this);

                        //destroy this gameObject
                        alreadyUsed = true;
                        Destroy(gameObject);
                    }
                }
            }
        }

        IEnumerator AutoDestruction()
        {
            //wait, then destroy
            yield return new WaitForSeconds(timeBeforeDestroy);
            alreadyUsed = true;
            Destroy(gameObject);
        }
    }
}