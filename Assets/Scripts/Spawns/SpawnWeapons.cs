using System.Collections.Generic;
using UnityEngine;
using redd096;

public class SpawnWeapons : MonoBehaviour
{
    [SerializeField] [Min(1)] int numberOfWeaponsToSpawn = 1;
    [SerializeField] WeaponBASE[] weaponsPrefabs = default;
    [SerializeField] Transform[] spawnPoints = default;

    void Start()
    {
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
        List<WeaponBASE> availableWeaponsPrefabs = new List<WeaponBASE>(weaponsPrefabs);
        WeaponBASE weaponPrefabToSpawn;
        for (int i = 0; i < numberOfWeaponsToSpawn; i++)
        {
            //break if there aren't available spawns points left
            if (availableSpawnPoints == null || availableSpawnPoints.Count <= 0)
                break;

            //break if there aren't available weapons prefabs left
            if (availableWeaponsPrefabs == null || availableWeaponsPrefabs.Count <= 0)
                break;

            //get random weapon prefab and remove from the list
            weaponPrefabToSpawn = availableWeaponsPrefabs[Random.Range(0, availableWeaponsPrefabs.Count)];
            availableWeaponsPrefabs.Remove(weaponPrefabToSpawn);

            if (weaponPrefabToSpawn)
            {
                //get random spawn point and remove from the list
                Transform spawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
                availableSpawnPoints.Remove(spawnPoint);

                //instantiate weapon at spawn points
                if (spawnPoint) Instantiate(weaponPrefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}
