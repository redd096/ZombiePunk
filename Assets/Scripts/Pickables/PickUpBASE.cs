using System.Collections;
using UnityEngine;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Pickables/Pick Up BASE")]
    public abstract class PickUpBASE : MonoBehaviour, IPickable
    {
        [Header("Destroy when instantiated - 0 = no destroy")]
        [SerializeField] float timeBeforeDestroy = 0;

        [Header("Magnet")]
        [SerializeField] bool canBePickedWithMagnet = true;
        [EnableIf("canBePickedWithMagnet")] [SerializeField] string magnetName = "CoinMagnet";
        [EnableIf("canBePickedWithMagnet")] [SerializeField] float magnetspeed = 7;
        [Tooltip("Move with rigidbody or transform?")] [EnableIf("canBePickedWithMagnet")] [SerializeField] bool moveWithRigidbody = true;

        //events
        public System.Action<PickUpBASE> onPick { get; set; }
        public System.Action<PickUpBASE> onFailPick { get; set; }

        protected Character whoHit;
        bool alreadyUsed;

        //magnet
        Rigidbody2D rb;
        protected GameObject player;
        Fluctuate fluctuate;

        protected virtual void OnEnable()
        {
            //reset vars
            alreadyUsed = false;

            //get references
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (fluctuate == null) fluctuate = GetComponent<Fluctuate>();

            //if there is, start auto destruction timer
            if (timeBeforeDestroy > 0)
                StartCoroutine(AutoDestruction());
        }

        protected virtual void FixedUpdate()
        {
            if (alreadyUsed)
                return;

            //if trigger enter and can't pick up, recall to not have object compenetrate
            if (whoHit && CanPickUp())
                PickUp();

            //move to player
            if (player && CanPickUp())
            {
                //disable fluctuate
                if (fluctuate && fluctuate.enabled) fluctuate.enabled = false;

                //with rigidbody or transform
                Vector2 playerdirection = (player.transform.position - transform.position).normalized;
                if (moveWithRigidbody)
                {
                    if (rb) rb.velocity = magnetspeed * playerdirection;
                }
                else
                {
                    transform.position += magnetspeed * (Vector3)playerdirection * Time.fixedDeltaTime;
                }
            }
            //else re-enable fluctuate
            else if (fluctuate && fluctuate.enabled == false)
            {
                fluctuate.enabled = true;
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (alreadyUsed)
                return;

            //on hit CoinMagnet, move to player if necessary
            if (canBePickedWithMagnet && collision.gameObject.name.Contains(magnetName))
            {
                player = GameManager.instance && GameManager.instance.levelManager && GameManager.instance.levelManager.Players != null && GameManager.instance.levelManager.Players.Count > 0 ? 
                    GameManager.instance.levelManager.Players[0].gameObject : 
                    GameObject.Find("Player");
            }

            //if hitted by player
            Character ch = collision.transform.GetComponentInParent<Character>();
            if (ch && ch.CharacterType == Character.ECharacterType.Player)
            {
                whoHit = ch;

                //pick up
                PickUp();
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            //when exit from magnet, if can't pick up then remove player
            if (CanPickUp() == false)
            {
                player = null;
            }

            //remove who hit on trigger exit
            if (whoHit && collision.transform.GetComponentInParent<Character>() == whoHit)
                whoHit = null;
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

        protected abstract bool CanPickUp();

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