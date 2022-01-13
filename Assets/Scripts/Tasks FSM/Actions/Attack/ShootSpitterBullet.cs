using UnityEngine;
using redd096;
using NaughtyAttributes;

[AddComponentMenu("redd096/Tasks FSM/Action/Attack/Shoot Spitter Bullet")]
public class ShootSpitterBullet : ActionTask
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] AimComponent aimComponent;

    [Header("Weapon - default barrel is this transform")]
    [SerializeField] Transform barrel = default;
    [SerializeField] Bullet bulletPrefab = default;
    [SerializeField] float damage = default;
    [SerializeField] float bulletSpeed = default;

    [Header("Shoot")]
    [SerializeField] string targetBlackboardName = "Target";
    [SerializeField] float timeBeforeFirstAttack = 0.5f;

    [Header("Repeat Attack")]
    [SerializeField] bool repeatAttack = false;
    [EnableIf("repeatAttack")] [SerializeField] float timeBetweenAttacks = 0.2f;

    [Header("DEBUG")]
    [SerializeField] bool drawDebug = false;

    Character selfCharacter;
    Transform target;
    float timerBeforeAttack;    //time between attacks

    //bullets pooling
    Pooling<Bullet> bulletsPooling = new Pooling<Bullet>();
    GameObject bulletsParent;
    GameObject BulletsParent
    {
        get
        {
            if (bulletsParent == null)
                bulletsParent = new GameObject("Bullets Parent");

            return bulletsParent;
        }
    }

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //get references
        if (aimComponent == null) aimComponent = GetStateMachineComponent<AimComponent>();
        if (barrel == null) barrel = transform;
        selfCharacter = GetStateMachineComponent<Character>();
    }

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //get target from blackboard
        target = stateMachine.GetBlackboardElement(targetBlackboardName) as Transform;

        //set timer for next attack
        timerBeforeAttack = Time.time + timeBeforeFirstAttack;
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
        if (target == null || bulletPrefab == null)
            return;

        //direction from barrel to target
        Vector2 direction = (target.position - barrel.position).normalized;

        //draw debug
        if (drawDebug)
            Debug.DrawLine(barrel.position, target.position, Color.red, 1);

        //instantiate bullet
        Bullet bullet = bulletsPooling.Instantiate(bulletPrefab, BulletsParent.transform);
        bullet.transform.position = barrel.position;
        bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * direction);    //rotate direction to left, to use right as forward

        //and set it (with delay autodestruction when reach target position)
        bullet.Init(selfCharacter, direction, damage, bulletSpeed, Vector2.Distance(barrel.position, target.position) / bulletSpeed );
    }
}
