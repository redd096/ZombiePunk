﻿using System.Collections;
using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Pickables/Pick Up BASE")]
    public abstract class PickUpBASE : MonoBehaviour, IPickable
    {
        [Header("Destroy when instantiated - 0 = no destroy")]
        [SerializeField] float timeBeforeDestroy = 0;

        [Header("Magnet")]
        [SerializeField] bool canBePickedWithMagnet = true;
        [SerializeField] float magnetspeed = 7;

        //events
        public System.Action<PickUpBASE> onPick { get; set; }
        public System.Action<PickUpBASE> onFailPick { get; set; }

        protected Character whoHit;
        bool alreadyUsed;

        //magnet
        Rigidbody2D rb;
        GameObject player;

        protected virtual void OnEnable()
        {
            //reset vars
            alreadyUsed = false;

            //get references
            if (rb == null) rb = GetComponent<Rigidbody2D>();

            //if there is, start auto destruction timer
            if (timeBeforeDestroy > 0)
                StartCoroutine(AutoDestruction());
        }

        protected virtual void FixedUpdate()
        {
            //move to player
            if (player)
            {
                Vector2 playerdirection = (player.transform.position - transform.position).normalized;
                if (rb) rb.velocity = magnetspeed * playerdirection;
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (alreadyUsed)
                return;

            //on hit CoinMagnet, move to player if necessary
            if (canBePickedWithMagnet && collision.gameObject.name.Contains("CoinMagnet"))
            {
                player = GameManager.instance && GameManager.instance.levelManager && GameManager.instance.levelManager.Players != null && GameManager.instance.levelManager.Players.Count > 0 ? 
                    GameManager.instance.levelManager.Players[0].gameObject : 
                    GameObject.Find("Player");

                //disable fluctuate script
                if (GetComponent<Fluctuate>())
                    GetComponent<Fluctuate>().enabled = false;
            }

            //if hitted by player
            whoHit = collision.transform.GetComponentInParent<Character>();
            if (whoHit && whoHit.CharacterType == Character.ECharacterType.Player)
            {
                //pick up
                PickUp();
            }
        }

        IEnumerator AutoDestruction()
        {
            //wait, then destroy
            yield return new WaitForSeconds(timeBeforeDestroy);
            alreadyUsed = true;
            Destroy(gameObject);
        }

        #region protected API

        public abstract void PickUp();

        protected virtual void OnPick()
        {
            //call event
            onPick?.Invoke(this);

            //destroy this gameObject
            alreadyUsed = true;
            Destroy(gameObject);
        }

        protected virtual void OnFailPick()
        {
            //call event
            onFailPick?.Invoke(this);
        }

        #endregion
    }
}