using System.Collections.Generic;
using UnityEngine;
using redd096;
using redd096.Attributes;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Attack/Damage In Area")]
public class DamageInArea : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] AimComponent component;

    [Header("Attack")]
    [SerializeField] float timeBeforeFirstAttack = 0.2f;
    [SerializeField] float damage = 10;
    [SerializeField] float pushForce = 10;

    [Header("Area Attack")]
    [SerializeField] string targetBlackboardName = "Target";
    [SerializeField] LayerMask targetLayer;
    [Tooltip("Check if there is a wall between this object and target")] [SerializeField] bool checkViewClear = true;
    [EnableIf("checkViewClear")] [SerializeField] LayerMask layerWalls;
    [SerializeField] float offsetArea = 1;
    [SerializeField] Vector2 sizeArea = Vector2.one;

    [Header("Repeat Attack")]
    [SerializeField] bool repeatAttack = false;
    [EnableIf("repeatAttack")] [SerializeField] float timeBetweenAttacks = 0.2f;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    Character selfCharacter;
    float timerBeforeAttack;    //time between attacks
    List<Redd096Main> possibleTargets = new List<Redd096Main>();

    void OnDrawGizmos()
    {
        if (drawDebug)
        {
            Gizmos.color = Color.red;

            //draw attack area
            Vector2 direction = Application.isPlaying && component ? component.AimDirectionInput : Vector2.right;
            Gizmos.DrawWireCube((Vector2)transformTask.position + direction * offsetArea, sizeArea);

            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (component == null) component = GetStateMachineComponent<AimComponent>();
        selfCharacter = GetStateMachineComponent<Character>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //set timer before attack
        timerBeforeAttack = Time.time + timeBeforeFirstAttack;

        //aim at target
        Transform target = stateMachine.GetBlackboardElement<Transform>(targetBlackboardName);  //get target from blackboard
        if (component && target) component.AimAt(target.position);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        //wait before attack
        if (timerBeforeAttack > Time.time)
            return;

        if (timerBeforeAttack > 0)
        {
            //attack
            DoAttack();

            //set timer for next attack, or stop attacks (set timer -1)
            timerBeforeAttack = repeatAttack ? Time.time + timeBetweenAttacks : -1;
        }
    }

    void DoAttack()
    {
        //find possible targets
        FindPossibleTargets();
        foreach(Redd096Main target in new List<Redd096Main>(possibleTargets))
        {
            //if view is clear
            if(target && IsViewClear(target.transform))
            {
                //do damage and push back
                if (target.GetSavedComponent<HealthComponent>())
                    target.GetSavedComponent<HealthComponent>().GetDamage(damage, selfCharacter, transformTask.position);

                if (target && target.GetSavedComponent<MovementComponent>())
                    target.GetSavedComponent<MovementComponent>().PushInDirection((target.transform.position - transformTask.position).normalized, pushForce);
            }
        }
    }

    #region private API

    void FindPossibleTargets()
    {
        //clear list
        possibleTargets.Clear();

        //find every element in area, using layer
        Redd096Main target;
        Vector2 direction = component ? component.AimDirectionInput : Vector2.right;
        foreach (Collider2D col in Physics2D.OverlapBoxAll((Vector2)transformTask.position + direction * offsetArea, sizeArea, 0, targetLayer.value))
        {
            //add to list if has component
            target = col.GetComponentInParent<Redd096Main>();
            if (target && possibleTargets.Contains(target) == false)
            {
                possibleTargets.Add(target);
            }
        }
    }

    bool IsViewClear(Transform t)
    {
        //check there is nothing between
        return checkViewClear == false || Physics2D.Linecast(transformTask.position, t.position, layerWalls.value) == false;
    }

    #endregion
}
