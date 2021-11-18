using System.Collections;
using UnityEngine;
using redd096;

public class WeaponRange : WeaponBASE
{
    [Header("Range Weapon")]
    [Tooltip("Keep pressed or click?")] public bool Automatic = true;
    public float RateOfFire = 0.2f;
    [Tooltip("Push back when shoot")] public float Recoil = 1;
    [Tooltip("Rotate random the shot when instantiated")] float NoiseAccuracy = 10;

    [Header("Barrel")]
    [Tooltip("Spawn bullets")] public Transform[] Barrels = default;
    [Tooltip("When more than one barrel, shoot every bullet simultaneously or from one random?")] bool barrelSimultaneously = true;

    [Header("Bullet")]
    public Bullet bulletPrefab = default;
    public float damage = 10;
    public float bulletSpeed = 10;

    float timeForNextShot;
    Coroutine automaticShootCoroutine;

    //bullets
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

    //events
    public System.Action<Transform> onInstantiateBullet { get; set; }
    public System.Action onShoot { get; set; }
    public System.Action onPressAttack { get; set; }
    public System.Action onReleaseAttack { get; set; }

    public override void PressAttack()
    {
        //check rate of fire
        if (Time.time > timeForNextShot)
        {
            timeForNextShot = Time.time + RateOfFire;

            //shoot
            Shoot();

            //start coroutine if automatic
            if (Automatic)
            {
                if (automaticShootCoroutine != null)
                    StopCoroutine(automaticShootCoroutine);

                automaticShootCoroutine = StartCoroutine(AutomaticShootCoroutine());
            }

            //call event
            onPressAttack?.Invoke();
        }
    }

    public override void ReleaseAttack()
    {
        //stop coroutine if running (automatic shoot)
        if (automaticShootCoroutine != null)
            StopCoroutine(automaticShootCoroutine);

        //call event
        onReleaseAttack?.Invoke();
    }

    #region private API

    /// <summary>
    /// Create bullet from every barrel or one random barrel
    /// </summary>
    void Shoot()
    {
        //shoot every bullet
        if (barrelSimultaneously)
        {
            foreach (Transform barrel in Barrels)
            {
                InstantiateBullet(barrel);
            }
        }
        //or shoot one bullet from random barrel
        else
        {
            Transform barrel = Barrels[Random.Range(0, Barrels.Length)];
            InstantiateBullet(barrel);
        }

        //pushback character
        Owner.GetSavedComponent<MovementComponent>().PushInDirection(-Owner.GetSavedComponent<AimComponent>().AimDirectionInput, Recoil);

        //call event
        onShoot?.Invoke();
    }

    /// <summary>
    /// Instantiate bullet and set it
    /// </summary>
    /// <param name="barrel"></param>
    void InstantiateBullet(Transform barrel)
    {
        //create random noise in accuracy
        float randomNoiseAccuracy = Random.Range(-NoiseAccuracy, NoiseAccuracy);
        Vector2 direction = Quaternion.AngleAxis(randomNoiseAccuracy, Vector3.forward) * barrel.right;                                  //direction with noise

        Debug.DrawLine(barrel.position, (Vector2)barrel.position + direction, Color.red, 1);

        //instantiate bullet
        Bullet bullet = bulletsPooling.Instantiate(bulletPrefab, BulletsParent.transform);
        bullet.transform.position = barrel.position;
        bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * direction);    //rotate direction to left, to use right as forward

        //and set it
        bullet.Init(this, Owner, direction);

        //call event
        onInstantiateBullet?.Invoke(barrel);
    }

    /// <summary>
    /// Continue shooting
    /// </summary>
    /// <returns></returns>
    IEnumerator AutomaticShootCoroutine()
    {
        while (true)
        {
            //stop if lose owner
            if (Owner == null)
                break;

            //check rate of fire
            if (Time.time > timeForNextShot)
            {
                timeForNextShot = Time.time + RateOfFire;

                //shoot
                Shoot();
            }

            yield return null;
        }
    }

    #endregion
}
