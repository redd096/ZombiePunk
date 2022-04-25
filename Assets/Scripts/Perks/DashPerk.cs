using UnityEngine;
using redd096.GameTopDown2D;

[CreateAssetMenu(menuName = "Zombie Punk/Dash Perk")]
public class DashPerk : PerkData
{
    [Header("Dash (to aim or movement direction?)")]
    [Tooltip("Push to Aim Direction, or Movement Direction?")] [SerializeField] bool dashToAimDirection = false;
    [SerializeField] float dashForce = 20;
    [SerializeField] float dashDelay = 1;
    [SerializeField] float durationInvincible = 0.2f;

    float cooldownTime;

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


            return true;
        }

        return false;
    }
}
