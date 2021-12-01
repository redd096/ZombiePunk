using UnityEngine;
using redd096;

public class TempSelectWeapon : MonoBehaviour
{
    /// <summary>
    /// Set weapon for next scene (save in GameManager)
    /// </summary>
    /// <param name="weapon"></param>
    public void SetWeapon(WeaponBASE weapon)
    {
        GameManager.instance.SetWeapon(weapon);
    }
}
