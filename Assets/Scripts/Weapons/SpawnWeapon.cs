using UnityEngine;
using redd096;

public class SpawnWeapon : MonoBehaviour
{
    [SerializeField] WeaponBASE weaponPrefab = default;

    //editor
    [HideInInspector] [SerializeField] SpriteRenderer spriteInEditor = default;

    void OnValidate()
    {
        //only in editor
        if (Application.isPlaying == false)
        {
            //add sprite renderer
            if (spriteInEditor == null)
                spriteInEditor = gameObject.AddComponent<SpriteRenderer>();

            //set sprite weapon
            spriteInEditor.sprite = weaponPrefab ? weaponPrefab.WeaponSprite : null;
        }
    }

    void Awake()
    {
        //remove sprite renderer in game
        if (spriteInEditor)
            Destroy(spriteInEditor);

        if (weaponPrefab)
        {
            //instantiate weapon and save prefab
            WeaponBASE weapon = Instantiate(weaponPrefab, transform.position, transform.rotation);
            weapon.WeaponPrefab = weaponPrefab;
        }
    }
}
