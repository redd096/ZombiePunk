using System.Collections;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;
using redd096.Attributes;

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

    [Header("Stop shooting during dash")]
    [SerializeField] bool stopShooting = true;
    [EnableIf("stopShooting")] [SerializeField] bool hideWeapon = true;
    [EnableIf("stopShooting")] [SerializeField] float durationStopShooting = 1;

    float cooldownTime;
    Coroutine untouchableCoroutine;
    Coroutine stopShootingCoroutine;
    bool[] ignoredLayers;

    public override float GetPerkDeltaCooldown() => 1 - (cooldownTime - Time.time) / dashDelay;

    public override void Equip(Redd096Main owner)
    {
        //set owner
        base.Equip(owner);

        //reset vars (because in scriptable object will remain saved also if not serialized)
        cooldownTime = 0;
        if (untouchableCoroutine != null && owner) owner.StopCoroutine(untouchableCoroutine);
        if (stopShootingCoroutine != null && owner) owner.StopCoroutine(stopShootingCoroutine);
        ResetLayers();
    }

    public override void Unequip()
    {
        //reset vars (because in scriptable object will remain saved also if not serialized)
        cooldownTime = 0;
        if (untouchableCoroutine != null && owner) owner.StopCoroutine(untouchableCoroutine);
        if (stopShootingCoroutine != null && owner) owner.StopCoroutine(stopShootingCoroutine);
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
            if (durationUntouchable > Mathf.Epsilon)
            {
                if (untouchableCoroutine != null) owner.StopCoroutine(untouchableCoroutine);
                ResetLayers();
                untouchableCoroutine = owner.StartCoroutine(UntouchableCoroutine());
            }

            //start StopShooting coroutine
            if (stopShooting && durationStopShooting > Mathf.Epsilon)
            {
                if (stopShootingCoroutine != null) owner.StopCoroutine(stopShootingCoroutine);
                stopShootingCoroutine = owner.StartCoroutine(StopShootingCoroutine());
            }

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
            if (layersToIgnore.ContainsLayer(i) && owner && Physics2D.GetIgnoreLayerCollision(owner.gameObject.layer, i) == false)
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

    IEnumerator StopShootingCoroutine()
    {
        WeaponBASE currentWeapon = null;

        //disable Attack and SwitchWeapon Input
        if (owner)
        {
            foreach (AttackByInput comp in owner.GetComponentsInChildren<AttackByInput>())
                comp.enabled = false;
            foreach (SwitchWeaponByInput comp in owner.GetComponentsInChildren<SwitchWeaponByInput>())
                comp.enabled = false;
        }

        //deactive current weapon
        if (hideWeapon)
        {
            if (owner.GetSavedComponent<WeaponComponent>() && owner.GetSavedComponent<WeaponComponent>().CurrentWeapon)
            {
                currentWeapon = owner.GetSavedComponent<WeaponComponent>().CurrentWeapon;
                currentWeapon.gameObject.SetActive(false);
            }
        }

        //wait
        yield return new WaitForSeconds(durationStopShooting);

        //reactive current weapon
        if (hideWeapon)
        {
            if (currentWeapon)
                currentWeapon.gameObject.SetActive(true);
        }

        //wait to update position before start shoot
        yield return null;

        //re-enable Attack and SwitchWeapon Input
        if (owner)
        {
            foreach (AttackByInput comp in owner.GetComponentsInChildren<AttackByInput>())
                comp.enabled = true;
            foreach (SwitchWeaponByInput comp in owner.GetComponentsInChildren<SwitchWeaponByInput>())
                comp.enabled = true;
        }
    }

    void ResetLayers()
    {
        if (ignoredLayers != null && owner)
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
