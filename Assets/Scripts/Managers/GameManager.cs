using UnityEngine.SceneManagement;
using System.Collections.Generic;
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
    [SerializeField] WeaponBASE[] currentWeaponsPrefabs = default;

    public UIManager uiManager { get; private set; }
    public LevelManager levelManager { get; private set; }

    List<Character> playersFromPreviousScene;

    protected override void SetDefaults()
    {
        //get references
        uiManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();

        //lock 60 fps or free
        Application.targetFrameRate = lock60Fps ? 60 : -1;

        //remove players already in scene, if we have moved some player from previous scene to this one
        RemovePlayersAlreadyInScene();
    }

    void OnValidate()
    {
        //lock 60 fps or free
        Application.targetFrameRate = lock60Fps ? 60 : -1;
    }

    #region move players between scenes

    public void MovePlayersToNextScene(Character[] players)
    {
        //save players list
        playersFromPreviousScene = new List<Character>(players);

        //set every player to DontDestroyOnLoad
        foreach (Character player in playersFromPreviousScene)
        {
            DontDestroyOnLoad(player);
            player.gameObject.SetActive(false);

            //set also equipped weapons to DontDestroyOnLoad
            if (player.GetSavedComponent<WeaponComponent>())
            {
                DontDestroyOnLoad(player.GetSavedComponent<WeaponComponent>().WeaponsParent.gameObject);
                player.GetSavedComponent<WeaponComponent>().WeaponsParent.gameObject.SetActive(false);
            }
        }
    }

    public void RemovePlayersAlreadyInScene()
    {
        //do only if already moved someone from previous scene
        if (playersFromPreviousScene == null)
            return;

        List<Vector2> positionsPlayers = new List<Vector2>();

        foreach (Character player in FindObjectsOfType<Character>())
        {
            //foreach player in scene
            if (player.CharacterType == Character.ECharacterType.Player)
            {
                //if not in the list, destroy, because is a player already in scene (put in editor)
                if (playersFromPreviousScene.Contains(player) == false)
                {
                    positionsPlayers.Add(player.transform.position);    //but save position
                    player.gameObject.SetActive(false);                 //and avoid this to call Awake and create for example its default weapon
                    Destroy(player.gameObject);
                }
            }
        }

        //foreach player in the list
        foreach (Character player in playersFromPreviousScene)
        {
            //remove DontDestroyOnLoad and set position
            SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetActiveScene());
            player.transform.position = positionsPlayers[Random.Range(0, positionsPlayers.Count)];
            player.gameObject.SetActive(true);

            //remove DontDestroyOnLoad also from equipped weapons
            if (player.GetSavedComponent<WeaponComponent>())
            {
                SceneManager.MoveGameObjectToScene(player.GetSavedComponent<WeaponComponent>().WeaponsParent.gameObject, SceneManager.GetActiveScene());
                player.GetSavedComponent<WeaponComponent>().WeaponsParent.gameObject.SetActive(true);
            }
        }

        //reset var, so will be saved only if moving again from this scene to another one
        playersFromPreviousScene = null;
    }

    #endregion

    #region OLD customizations API

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

    #region weapons API

    /// <summary>
    /// Set weapons for next scene
    /// </summary>
    /// <param name="weaponsPrefabs"></param>
    public void SetWeapons(WeaponBASE[] weaponsPrefabs)
    {
        currentWeaponsPrefabs = weaponsPrefabs;
    }

    /// <summary>
    /// Return current weapons
    /// </summary>
    /// <returns></returns>
    public WeaponBASE[] GetWeapons()
    {
        return currentWeaponsPrefabs;
    }

    /// <summary>
    /// Return if there are weapons saved
    /// </summary>
    /// <returns></returns>
    public bool HasWeaponsSaved()
    {
        return currentWeaponsPrefabs != null && currentWeaponsPrefabs.Length > 0;
    }

    /// <summary>
    /// Reset weapons
    /// </summary>
    public void ClearWeapons()
    {
        currentWeaponsPrefabs = null;
    }

    #endregion
}