using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Singletons/Game Manager")]
[DefaultExecutionOrder(-100)]
public class GameManager : Singleton<GameManager>
{
    [Header("Lock 60 FPS")]
    [SerializeField] bool lock60Fps = true;

    //se si vuole fare multiplayer, si salva un array per ogni ID
    //nel menu customizzazione si aggiunge uno script ai prefab per passare il PointerEventData al click, per sapere l'ID di chi ha cliccato
    [Header("DEBUG")]
    [SerializeField] CustomizeData[] currentCustomizations = default;

    public UIManager uiManager { get; private set; }
    public PathFindingAStar2D pathFindingAStar { get; private set; }
    public LevelManager levelManager { get; private set; }

    protected override void SetDefaults()
    {
        //get references
        uiManager = FindObjectOfType<UIManager>();
        pathFindingAStar = FindObjectOfType<PathFindingAStar2D>();
        levelManager = FindObjectOfType<LevelManager>();

        //lock 60 fps or free
        Application.targetFrameRate = lock60Fps ? 60 : -1;
    }

    #region public API

    /// <summary>
    /// Set customizations for next scene
    /// </summary>
    /// <param name="customizations"></param>
    public void SetCustomizations(CustomizeData[] customizations)
    {
        currentCustomizations = customizations;
    }

    /// <summary>
    /// Return current list of customizations
    /// </summary>
    /// <returns></returns>
    public CustomizeData[] GetCustomizations()
    {
        return currentCustomizations;
    }

    /// <summary>
    /// Reset customizations
    /// </summary>
    public void ClearCustomizations()
    {
        currentCustomizations = new CustomizeData[0];
    }

    /// <summary>
    /// Add customizations to weapon
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public WeaponBASE AddCustomizationsToWeapon(WeaponBASE weapon)
    {
        //if there are customizations and is a range weapon
        if (currentCustomizations != null && currentCustomizations.Length > 0 && weapon is WeaponRange weaponRange)
        {
            //add every customization
            for (int i = 0; i < currentCustomizations.Length; i++)
            {
                if (currentCustomizations[i])
                    weaponRange = currentCustomizations[i].AddRangeCustomizations(weaponRange);
            }

            return weaponRange;
        }

        //else return normal weapon
        return weapon;
    }

    #endregion
}