using System.Collections;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[CreateAssetMenu(menuName = "Zombie Punk/Dash Perk")]
public class DashPerk : PerkData
{
    [Header("Dash (to aim or movement direction?)")]
    [Tooltip("Push to Aim Direction, or Movement Direction?")] [SerializeField] bool dashToAimDirection = false;
    [SerializeField] float dashForce = 20;
    [SerializeField] float dashDelay = 1;
    [SerializeField] float durationInvincible = 0.2f;

    [Header("Ignore collisions with some layers")]
    [SerializeField] float durationUntouchable = 0;
    [SerializeField] LayerMask layersToIgnore = default;

    float cooldownTime;
    Coroutine untouchableCoroutine;
    bool[] ignoredLayers;

    public override float GetPerkDeltaCooldown() => 1 - (cooldownTime - Time.time) / dashDelay;

    public override void Equip(Redd096Main owner)
    {
        //reset vars (because in scriptable object will remain saved also if not serialized)
        cooldownTime = 0;

        //set owner
        base.Equip(owner);
    }

    public override void Unequip()
    {
        //reset vars (because in scriptable object will remain saved also if not serialized)
        cooldownTime = 0;
        if (untouchableCoroutine != null && owner) owner.StopCoroutine(untouchableCoroutine);
        ResetLayers();

        //remove owner
        base.Unequip();
    }

    public override bool UsePerk()
    {
        if (owner == null || owner.GetSavedComponent<MovementComponent>() == null)
            return false;

        //check cooldown
        if (Time.time > cooldownTime)
        {
            cooldownTime = Time.time + dashDelay;
        
            //add as push in Aim Direction
            if (dashToAimDirection)
            {
                if (owner.GetSavedComponent<AimComponent>())
                    owner.GetSavedComponent<MovementComponent>().PushInDirection(owner.GetSavedComponent<AimComponent>().AimDirectionInput, dashForce);
            }
            //or push in Movement Direction
            else
            {
                //if moving, push in direction - if still, push right or left based on where is aiming
                if (owner.GetSavedComponent<MovementComponent>().MoveDirectionInput != Vector2.zero)
                    owner.GetSavedComponent<MovementComponent>().PushInDirection(owner.GetSavedComponent<MovementComponent>().MoveDirectionInput, dashForce);
                else if (owner.GetSavedComponent<AimComponent>())
                    owner.GetSavedComponent<MovementComponent>().PushInDirection(owner.GetSavedComponent<AimComponent>().IsLookingRight ? Vector2.right : Vector2.left, dashForce);
            }

            //set temporary invincible
            if (owner.GetSavedComponent<HealthComponent>())
            {
                owner.GetSavedComponent<HealthComponent>().SetTemporaryInvincible(durationInvincible);
            }

            //start untouchable coroutine
            if (untouchableCoroutine != null) owner.StopCoroutine(untouchableCoroutine);
            untouchableCoroutine = owner.StartCoroutine(UntouchableCoroutine());


            return true;
        }

        return false;
    }

    IEnumerator UntouchableCoroutine()
    {
        ignoredLayers = new bool[32];

        for (int i = 0; i < 32; i++)
        {
            //if layer to ignore and is not already ignored by owner
            if (layersToIgnore.ContainsLayer(i) && Physics2D.GetIgnoreLayerCollision(owner.gameObject.layer, i) == false)
            {
                //save and set ignore layer
                ignoredLayers[i] = true;
                Physics2D.IgnoreLayerCollision(owner.gameObject.layer, i, true);
            }
        }

        //wait
        yield return new WaitForSeconds(durationUntouchable);

        //reset collisions
        ResetLayers();
    }

    void ResetLayers()
    {
        if (ignoredLayers != null)
        {
            //cycle every ignored layer
            for (int i = 0; i < ignoredLayers.Length; i++)
            {
                if (ignoredLayers[i])
                {
                    //and reset ignore collision
                    Physics2D.IgnoreLayerCollision(owner.gameObject.layer, i, false);
                }
            }

            ignoredLayers = null;
        }
    }
}
