using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Use trigger or collision enter")]
    [SerializeField] bool useTrigger = true;

    [Header("Layer Penetrable")]
    [SerializeField] LayerMask layerPenetrable = default;

    [Header("Bullet")]
    [SerializeField] bool ignoreShield = false;
    [Tooltip("Knockback who hit")] [SerializeField] float knockBack = 1;

    [Header("Area Damage")]
    [SerializeField] bool doAreaDamage = false;
    [EnableIf("doAreaDamage")] [SerializeField] bool ignoreShieldAreaDamage = false;
    [Tooltip("Damage others in radius area")] [EnableIf("doAreaDamage")] [SerializeField] [Min(0)] float radiusAreaDamage = 0;
    [Tooltip("Is possible to damage owner with area damage?")] [EnableIf("doAreaDamage")] [SerializeField] bool areaCanDamageWhoShoot = false;
    [Tooltip("Is possible to damage again who hit this bullet?")] [EnableIf("doAreaDamage")] [SerializeField] bool areaCanDamageWhoHit = false;
    [Tooltip("Do knockback also to who hit in area?")] [EnableIf("doAreaDamage")] [SerializeField] bool knockbackAlsoInArea = true;

    [Header("Timer Autodestruction (0 = no autodestruction)")]
    [SerializeField] public float delayAutodestruction = 0;
    [EnableIf("doAreaDamage")] [SerializeField] bool doAreaDamageAlsoOnAutoDestruction = true;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] Vector2 direction = Vector2.zero;
    [ReadOnly] [SerializeField] float damage = 0;
    [ReadOnly] [SerializeField] float bulletSpeed = 0;

    Rigidbody2D rb;
    Redd096Main owner;
    WeaponRange weapon;
    bool alreadyDead;
    List<Redd096Main> alreadyHit = new List<Redd096Main>();

    Coroutine autodestructionCoroutine;

    //events
    public System.Action<GameObject> onHit { get; set; }        //also when penetrate something
    public System.Action<GameObject> onLastHit { get; set; }    //when hit something that destroy this bullet
    public System.Action onAutodestruction { get; set; }
    public System.Action onDie { get; set; }

    private void Awake()
    {
        //get references
        rb = GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos()
    {
        //draw area damage
        if (doAreaDamage && radiusAreaDamage > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusAreaDamage);
        }
    }

    /// <summary>
    /// Initialize bullet
    /// </summary>
    /// <param name="weapon"></param>
    /// <param name="owner"></param>
    /// <param name="direction"></param>
    public void Init(WeaponRange weapon, Redd096Main owner, Vector2 direction)
    {
        //reset vars
        alreadyDead = false;
        alreadyHit.Clear();

        this.direction = direction;
        this.damage = weapon.damage;
        this.bulletSpeed = weapon.bulletSpeed;

        this.owner = owner;
        this.weapon = weapon;

        //ignore every collision with owner
        if (owner)
        {
            foreach (Collider2D ownerCol in owner.GetComponentsInChildren<Collider2D>())
                foreach (Collider2D bulletCol in GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(bulletCol, ownerCol);
        }

        //ignore every collision with weapon
        if (weapon)
        {
            foreach (Collider2D weaponCol in weapon.GetComponentsInChildren<Collider2D>())
                foreach (Collider2D bulletCol in GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(bulletCol, weaponCol);
        }

        //autodestruction coroutine
        if (delayAutodestruction > 0)
        {
            autodestructionCoroutine = StartCoroutine(AutoDestructionCoroutine());
        }
    }

    void FixedUpdate()
    {
        //move bullet
        rb.velocity = direction * bulletSpeed;
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

    #region private API

    void OnHit(GameObject hit)
    {
        if (alreadyDead)
            return;

        //be sure to hit something, but not other bullets or this weapon or this owner
        if (hit == null || hit.GetComponentInParent<Bullet>() || hit.GetComponentInParent<WeaponRange>() == weapon || hit.GetComponentInParent<Redd096Main>() == owner)
            return;

        //don't hit again same damageable (for penetrate shots)
        Redd096Main hitMain = hit.GetComponentInParent<Redd096Main>();
        if (hitMain && alreadyHit.Contains(hitMain))
            return;

        //call event
        onHit?.Invoke(hit);

        if (hitMain)
        {
            alreadyHit.Add(hitMain);

            //if hit something damageable, do damage and push back
            if (hitMain.GetSavedComponent<HealthComponent>()) hitMain.GetSavedComponent<HealthComponent>().GetDamage(damage);
            if (hitMain.GetSavedComponent<MovementComponent>()) hitMain.GetSavedComponent<MovementComponent>().PushInDirection(direction, knockBack);
        }

        //if is not a penetrable layer, destroy this object
        if (layerPenetrable.ContainsLayer(hit.layer) == false)
        {
            //call event
            onLastHit?.Invoke(hit);

            //damage in area too
            if (doAreaDamage && radiusAreaDamage > 0)
                DamageInArea(hitMain);

            //destroy
            Die();
        }
    }

    void DamageInArea(Redd096Main hit)
    {
        //be sure to not hit again the same
        List<Redd096Main> hits = new List<Redd096Main>();

        //be sure to not hit owner (if necessary)
        if (areaCanDamageWhoShoot == false && owner)
            hits.Add(owner);

        //be sure to not hit who was already hit by bullet (if necessary)
        if (areaCanDamageWhoHit == false && hit != null)
            hits.Add(hit);

        //find every object damageable in area
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, radiusAreaDamage))
        {
            Redd096Main hitMain = col.GetComponentInParent<Redd096Main>();
            if (hitMain != null && hits.Contains(hitMain) == false)
            {
                //add only one time in the list, and do damage
                hits.Add(hitMain);
                if(hitMain.GetSavedComponent<HealthComponent>()) hitMain.GetSavedComponent<HealthComponent>().GetDamage(damage);

                //and knockback
                if (knockbackAlsoInArea)
                {
                    if (hitMain.GetSavedComponent<MovementComponent>()) hitMain.GetSavedComponent<MovementComponent>().PushInDirection((col.transform.position - transform.position).normalized, knockBack);
                }
            }
        }
    }

    IEnumerator AutoDestructionCoroutine()
    {
        //wait
        yield return new WaitForSeconds(delayAutodestruction);

        //only if not already dead
        if (alreadyDead)
            yield break;

        //do damage in area too
        if (doAreaDamageAlsoOnAutoDestruction)
        {
            if (doAreaDamage && radiusAreaDamage > 0)
                DamageInArea(null);
        }

        //call event
        onAutodestruction?.Invoke();

        //then destroy
        Die();
    }

    #endregion

    void Die()
    {
        alreadyDead = true;

        //if coroutine is running, stop it
        if (autodestructionCoroutine != null)
            StopCoroutine(autodestructionCoroutine);

        //call event
        onDie?.Invoke();

        //destroy bullet
        Pooling.Destroy(gameObject);
    }
}
