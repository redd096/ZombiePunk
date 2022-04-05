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

    float timeBeforeNextDash;

    public override void Init()
    {
        //reset time (because in scriptable object will remain saved also if not serialized)
        timeBeforeNextDash = 0;
    }

    public override bool UsePerk(Character owner)
    {
        if (owner == null || owner.GetSavedComponent<MovementComponent>() == null)
        {
            return false;
        }

        //check delay between dash
        if (Time.time > timeBeforeNextDash)
        {
            timeBeforeNextDash = Time.time + dashDelay;
        
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
        
            //set invincible
            if (owner.GetSavedComponent<HealthComponent>())
                owner.GetSavedComponent<HealthComponent>().SetTemporaryInvincible(durationInvincible);

            return true;
        }

        return false;
    }
}
