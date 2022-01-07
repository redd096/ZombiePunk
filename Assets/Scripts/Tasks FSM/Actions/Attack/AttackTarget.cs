using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/Attack/Attack Target")]
public class AttackTarget : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] WeaponComponent component;
    [SerializeField] AimComponent aimComponent;

    [Header("Shoot")]
    [SerializeField] string targetBlackboardName = "Target";
    [SerializeField] float timeBeforeFirstAttack = 0.5f;
    [SerializeField] float durationAttack = 1;
    [SerializeField] float timeBetweenAttacks = 0.5f;

    Transform target;
    float timerBeforeAttack;    //time between attacks
    float timerAttack;          //duration attack
    bool isAttacking;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetStateMachineComponent<WeaponComponent>();
        if (aimComponent == null) aimComponent = GetStateMachineComponent<AimComponent>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //get target from blackboard
        target = stateMachine.GetBlackboardElement(targetBlackboardName) as Transform;

        //be sure is not attacking and set timer for next attack
        StopAttack();
        timerBeforeAttack = Time.time + timeBeforeFirstAttack;
    }

    public override void OnExitTask()
    {
        base.OnExitTask();

        //be sure to stop attack
        StopAttack();
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        //aim at target
        if (aimComponent && target)
            aimComponent.AimAt(target.position);

        //wait before attack
        if (timerBeforeAttack > Time.time)
            return;

        //if not attacking, start attack
        if(isAttacking == false)
        {
            StartAttack();

            //set duration attack
            timerAttack = Time.time + durationAttack;
        }
        //if attacking, wait timer then stop attack
        else if(Time.time > timerAttack)
        {
            StopAttack();

            //set time before next attack
            timerBeforeAttack = Time.time + timeBetweenAttacks;
        }
    }

    #region private API

    void StartAttack()
    {
        isAttacking = true;

        //if there is a weapon equipped, start attack
        if (component && component.CurrentWeapon)
            component.CurrentWeapon.PressAttack();
    }

    void StopAttack()
    {
        isAttacking = false;

        //if there is a weapon equipped, stop attack
        if (component && component.CurrentWeapon)
            component.CurrentWeapon.ReleaseAttack();
    }

    #endregion
}
