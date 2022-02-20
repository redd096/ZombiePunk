using UnityEngine;
using redd096;

public class SpawnWeapon : MonoBehaviour
{
    [SerializeField] WeaponBASE weaponPrefab = default;

    void Awake()
    {
        if (weaponPrefab)
        {
            //instantiate weapon and save prefab
            WeaponBASE weapon = Instantiate(weaponPrefab, transform.position, transform.rotation);
            weapon.WeaponPrefab = weaponPrefab;
        }
    }
}
