using System.Collections.Generic;
using UnityEngine;
using redd096;
using NaughtyAttributes;

[AddComponentMenu("redd096/Tasks FSM/Action/Attack/Damage At Position")]
public class DamageAtPosition : ActionTask
{
    [Header("Attack")]
    [SerializeField] float timeBeforeFirstAttack = 0.2f;
    [SerializeField] float damage = 10;
    [SerializeField] float pushForce = 10;

    [Header("Area Attack")]
    [SerializeField] string positionBlackboardName = "Last Target Position";
    [SerializeField] LayerMask targetLayer;
    [Tooltip("Check if there is a wall between this object and target")] [SerializeField] bool checkViewClear = true;
    [EnableIf("checkViewClear")] [SerializeField] LayerMask layerWalls;
    [SerializeField] Vector2 sizeArea = Vector2.one;

    [Header("Repeat Attack")]
    [SerializeField] bool repeatAttack = false;
    [EnableIf("repeatAttack")] [SerializeField] float timeBetweenAttacks = 0.2f;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    Character selfCharacter;
    float timerBeforeAttack;    //time between attacks
    List<Redd096Main> possibleTargets = new List<Redd096Main>();
    Vector2 positionFromBlackboard;

    void OnDrawGizmos()
    {
        if (drawDebug)
        {
            Gizmos.color = Color.red;

            //draw attack area
            Gizmos.DrawWireCube(positionFromBlackboard, sizeArea);

            Gizmos.color = Color.white;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        selfCharacter = GetStateMachineComponent<Character>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //set timer before attack
        timerBeforeAttack = Time.time + timeBeforeFirstAttack;

        //get values from blackboard
        positionFromBlackboard = stateMachine.GetBlackboardElement<Vector2>(positionBlackboardName);
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
        foreach (Redd096Main target in new List<Redd096Main>(possibleTargets))
        {
            //if view is clear
            if (target && IsViewClear(target.transform))
            {
                //do damage and push back
                if (target.GetSavedComponent<HealthComponent>())
                    target.GetSavedComponent<HealthComponent>().GetDamage(damage, selfCharacter, positionFromBlackboard);

                if (target && target.GetSavedComponent<MovementComponent>())
                    target.GetSavedComponent<MovementComponent>().PushInDirection(((Vector2)target.transform.position - positionFromBlackboard).normalized, pushForce);
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
        foreach (Collider2D col in Physics2D.OverlapBoxAll(positionFromBlackboard, sizeArea, 0, targetLayer.value))
        {
            //add to list if has component
            target = col.GetComponentInParent<Redd096Main>();
            if (target && possibleTargets.Contains(target) == false)
            {
                //be sure is not self
                if (target != selfCharacter)
                {
                    possibleTargets.Add(target);
                }
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
