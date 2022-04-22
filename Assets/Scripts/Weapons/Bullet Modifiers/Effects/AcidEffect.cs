using UnityEngine;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Weapons/Bullet Modifiers/Effects/Acid Effect")]
public class AcidEffect : EffectAreaBASE
{
    [Header("Damage")]
    [SerializeField] float damage = 10;
    [SerializeField] float pushForce = 10;
    [Tooltip("Necessary for on collision stay, to not call damage every frame")] [SerializeField] float delayBetweenAttacks = 1;

    protected override float delayOnTriggerStay => delayBetweenAttacks;

    protected override void OnHit(Collider2D collision, Redd096Main hit)
    {
        //if there is no hit, do nothing
        if (hit == null)
            return;

        //else, do damage and push back
        if (hit.GetSavedComponent<HealthComponent>())
            hit.GetSavedComponent<HealthComponent>().GetDamage(damage, owner, collision.ClosestPoint(transform.position));

        if (hit && hit.GetSavedComponent<MovementComponent>())
            hit.GetSavedComponent<MovementComponent>().PushInDirection((hit.transform.position - collision.transform.position).normalized, pushForce);
    }
}
