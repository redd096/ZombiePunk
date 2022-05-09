using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096.Attributes;

public class ExplodeOnDeath : MonoBehaviour
{
    [Header("Necessary Components - default get from this gameObject")]
    [Tooltip("To know when is dead")] [SerializeField] HealthComponent component;

    [Header("Area Damage")]
    [SerializeField] bool doAreaDamage = true;
    [Tooltip("Damage others in radius area")] [EnableIf("doAreaDamage")] [SerializeField] [Min(0)] float radiusAreaDamage = 5;
    [EnableIf("doAreaDamage")] [SerializeField] bool ignoreShield = false;
    [EnableIf("doAreaDamage")] [SerializeField] float damage = 10;
    [EnableIf("doAreaDamage")] [SerializeField] float knockBack = 0;

    [Header("Ignore")]
    [Tooltip("Layers to ignore (no hit and no destroy bullet)")] [EnableIf("doAreaDamage")] [SerializeField] LayerMask layersToIgnore = default;
    [Tooltip("Ignore trigger colliders")] [EnableIf("doAreaDamage")] [SerializeField] bool ignoreTriggers = true;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    Character selfCharacter;

    void OnDrawGizmos()
    {
        //draw area damage
        if (drawDebug)
        {
            if (doAreaDamage && radiusAreaDamage > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, radiusAreaDamage);
            }
        }
    }

    void OnEnable()
    {
        //get references
        if (component == null)
            component = GetComponent<HealthComponent>();
        if (selfCharacter == null)
            selfCharacter = GetComponent<Character>();

        if (component == null)
            Debug.LogWarning("Missing HealthComponent on " + name);

        //add events
        if(component)
        {
            component.onDie += OnDie;
        }
    }

    void OnDisable()
    {
        //remove events
        if (component)
        {
            component.onDie -= OnDie;
        }
    }

    void OnDie(HealthComponent whoDied, Character whoHit)
    {
        //do area damage if setted
        if(doAreaDamage && radiusAreaDamage > 0)
            DamageInArea();
    }

    void DamageInArea()
    {
        //be sure to not hit again the same
        List<Redd096Main> hits = new List<Redd096Main>();

        //find every object damageable in area
        Redd096Main hitMain;
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, radiusAreaDamage))
        {
            hitMain = col.GetComponentInParent<Redd096Main>();

            if (hitMain != null && hits.Contains(hitMain) == false                      //be sure hit something and is not already hitted
                && ContainsLayer(layersToIgnore, hitMain.gameObject.layer) == false     //be sure is not in layers to ignore
                && (ignoreTriggers == false || col.isTrigger == false))                 //be sure is not enabled ignoreTriggers, or is not trigger
            {
                hits.Add(hitMain);

                //do damage
                if (hitMain.GetSavedComponent<HealthComponent>()) 
                    hitMain.GetSavedComponent<HealthComponent>().GetDamage(damage, selfCharacter, transform.position, ignoreShield);

                //and knockback
                if (hitMain && hitMain.GetSavedComponent<MovementComponent>())
                    hitMain.GetSavedComponent<MovementComponent>().PushInDirection((col.transform.position - transform.position).normalized, knockBack);
            }
        }
    }

    bool ContainsLayer(LayerMask layerMask, int layerToCompare)
    {
        //if add layer to this layermask, and layermask remain equals, then layermask contains this layer
        return layerMask == (layerMask | (1 << layerToCompare));
    }
}
