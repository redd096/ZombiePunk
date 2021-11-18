using UnityEngine;

public class RotateWeaponFeedback : MonoBehaviour
{
    [Header("Sprites to flip - default get in children")]
    [SerializeField] SpriteRenderer[] spritesToFlip = default;

    [Header("Pivot - default is this transform")]
    [SerializeField] Transform objectPivot = default;
    [SerializeField] float offsetFromPlayer = 1;

    WeaponBASE weaponBASE;

    void OnEnable()
    {
        //get references
        weaponBASE = GetComponent<WeaponBASE>();
        if (spritesToFlip == null || spritesToFlip.Length <= 0) spritesToFlip = GetComponentsInChildren<SpriteRenderer>();
        if (objectPivot == null) objectPivot = transform;
    }

    void Update()
    {
        //rotate weapon with aim and set position
        RotateWeapon();
        MoveWeapon();
    }

    void RotateWeapon()
    {
        //rotate weapon with aim (using pivot)
        if (weaponBASE.Owner && weaponBASE.Owner.GetSavedComponent<AimComponent>())
        {
            Vector2 aimDirection = weaponBASE.Owner.GetSavedComponent<AimComponent>().AimDirectionInput;
            objectPivot.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * aimDirection);

            //when rotate to left, flip Y to not be upside down
            foreach(SpriteRenderer sprite in spritesToFlip)
                sprite.flipY = aimDirection.x < 0;
        }
    }

    void MoveWeapon()
    {
        //move to owner + offset
        if (weaponBASE.Owner && weaponBASE.Owner.GetSavedComponent<AimComponent>())
        {
            transform.position = (Vector2)weaponBASE.Owner.transform.position + (weaponBASE.Owner.GetSavedComponent<AimComponent>().AimDirectionInput * offsetFromPlayer);
        }
    }
}
