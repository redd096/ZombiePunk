using UnityEngine;
using redd096;
using NaughtyAttributes;

[AddComponentMenu("redd096/Singletons/Game Manager")]
[DefaultExecutionOrder(-100)]
public class GameManager : Singleton<GameManager>
{
    [Header("DEBUG")]
    [ReadOnly] [SerializeField] WeaponBASE currentWeapon = default;

    public UIManager uiManager { get; private set; }
    public PathFindingAStar pathFindingAStar { get; private set; }
    public LevelManager levelManager { get; private set; }

    protected override void SetDefaults()
    {
        //get references
        uiManager = FindObjectOfType<UIManager>();
        pathFindingAStar = FindObjectOfType<PathFindingAStar>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    /// <summary>
    /// Set weapon for next scene
    /// </summary>
    /// <param name="weapon"></param>
    public void SetWeapon(WeaponBASE weapon)
    {
        currentWeapon = weapon;
    }

    /// <summary>
    /// Get weapon from previous scene
    /// </summary>
    /// <returns></returns>
    public WeaponBASE GetWeapon()
    {
        return currentWeapon;
    }
}