using System.Collections.Generic;
using UnityEngine;
using redd096.GameTopDown2D;
using redd096;

public class SpitterFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] ShootSpitterBullet shootSpitterBulletInStateMachine = default;

    [Header("Object to instantiate on shoot")]
    [SerializeField] GameObject objectOnShoot = default;

    Dictionary<Bullet, GameObject> objectsToDeactivate = new Dictionary<Bullet, GameObject>();

    private void OnEnable()
    {
        //get references
        if (shootSpitterBulletInStateMachine == null)
        {
            Redd096Main main = GetComponentInParent<Redd096Main>();
            if (main) shootSpitterBulletInStateMachine = main.GetComponentInChildren<ShootSpitterBullet>();
        }

        //add events
        if (shootSpitterBulletInStateMachine)
        {
            shootSpitterBulletInStateMachine.onShoot += OnShootSpitter;
        }
    }

    private void OnDisable()
    {
        //remove events
        if (shootSpitterBulletInStateMachine)
        {
            shootSpitterBulletInStateMachine.onShoot -= OnShootSpitter;
        }
    }

    private void OnShootSpitter(Vector2 positionToReach, Bullet bullet)
    {
        //instantiate object on position to reach
        InstantiatedGameObjectStruct objectToInstantiate = new InstantiatedGameObjectStruct();
        objectToInstantiate.instantiatedGameObject = objectOnShoot;
        objectToInstantiate.timeAutodestruction = -1;
        GameObject go = InstantiateGameObjectManager.instance.Play(objectToInstantiate, positionToReach, Quaternion.identity);

        //add to dictionary and register to bullet destroy event
        objectsToDeactivate.Add(bullet, go);
        bullet.onDie += BulletOnDie;
    }

    void BulletOnDie(Bullet bullet)
    {
        //deactive object and remove from dictionary
        if (objectsToDeactivate.ContainsKey(bullet))
        {
            if (objectsToDeactivate[bullet])
                Pooling.Destroy(objectsToDeactivate[bullet]);

            objectsToDeactivate.Remove(bullet);
        }

        //unregister from event
        bullet.onDie -= BulletOnDie;
    }
}
