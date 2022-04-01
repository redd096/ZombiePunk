using System.Collections.Generic;
using UnityEngine;
using redd096.Attributes;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Interactables/Exit Interactable")]
    public class ExitInteractable : MonoBehaviour, IInteractable
    {
        [Header("Rules to Open")]
        [Tooltip("Check there are no enemies in scene")] [SerializeField] bool checkNoEnemiesInScene = true;
        [Tooltip("If there are spawn managers in scene, check when every spawn has finished to spawn (spawns with RestartWhenFinish will not be checked)")] [SerializeField] bool checkEverySpawnManager = true;
        [Tooltip("Check every player has weapon")] [SerializeField] bool checkEveryPlayerHasWeapon = true;

        [Header("On Interact")]
        [SerializeField] [Scene] string sceneToLoad = default;
        [SerializeField] int levelReach;

        [Header("DEBUG")]
        /*[ShowNonSerializedField]*/
        [SerializeField] [ReadOnly] bool isOpen;

        [Button("Force Exit", ButtonAttribute.EEnableType.PlayMode)] public void ForceExit() => ChangeExitState();

        public bool IsOpen => isOpen;
        public string SceneToLoad => sceneToLoad;

        //events
        public System.Action onOpen { get; set; }
        public System.Action onClose { get; set; }
        public System.Action<ExitInteractable> onInteract { get; set; }

        //necessary for checks
        List<SpawnableObject> enemies = new List<SpawnableObject>();
        List<Character> players = new List<Character>();
        List<SpawnManager> spawnManagers = new List<SpawnManager>();

        void OnEnable()
        {
            ActiveExit();
        }

        void OnDisable()
        {
            DeactiveExit();
        }

        #region public API

        /// <summary>
        /// Level Manager call it to activate (become interactable and start check if can open)
        /// </summary>
        public void ActiveExit()
        {
            //register to every enemy death
            SpawnableObject spawnableObject;
            foreach (Character enemy in GameManager.instance.levelManager.Enemies)
            {
                if (enemy)
                {
                    spawnableObject = enemy.gameObject.AddComponent<SpawnableObject>();
                    spawnableObject.onDeactiveObject += OnEnemyDie;
                    enemies.Add(spawnableObject);   //and add to the list
                }
            }

            //register to every player change weapon
            foreach (Character player in GameManager.instance.levelManager.Players)
            {
                if (player && player.GetSavedComponent<WeaponComponent>())
                {
                    player.GetSavedComponent<WeaponComponent>().onChangeWeapon += OnPlayerChangeWeapon;
                    players.Add(player);        //and add to the list
                }
            }

            //register to every spawn manager finish spawn (only if not restart when finish)
            foreach (SpawnManager spawnManager in GameManager.instance.levelManager.SpawnManagers)
            {
                if (spawnManager && spawnManagers.Contains(spawnManager) == false)
                {
                    if (spawnManager.RestartWhenFinish == false)
                    {
                        //add also a check if spawn manager is destroyed
                        spawnableObject = spawnManager.gameObject.AddComponent<SpawnableObject>();
                        spawnableObject.onDeactiveObject += OnSpawnManagerDestroyed;

                        spawnManager.onFinishToSpawn += OnFinishToSpawn;
                        spawnManagers.Add(spawnManager);    //and add to the list

                        //add also every enemy spawned
                        spawnManager.onEverySpawn += OnEverySpawn;
                    }
                }
            }

            //do checks
            DoCheck();
        }

        /// <summary>
        /// Level Manager call it to deactive (is not interactable and doesn't do checks)
        /// </summary>
        public void DeactiveExit()
        {
            //unregister from every enemy
            foreach (SpawnableObject enemy in enemies)
            {
                if (enemy)
                    enemy.onDeactiveObject -= OnEnemyDie;
            }
            enemies.Clear();

            //unregister from every player
            foreach (Character player in players)
            {
                if (player && player.GetSavedComponent<WeaponComponent>())
                    player.GetSavedComponent<WeaponComponent>().onChangeWeapon -= OnPlayerChangeWeapon;
            }
            players.Clear();

            //unregister from every spawn manager
            foreach (SpawnManager spawnManager in spawnManagers)
            {
                if (spawnManager)
                {
                    SpawnableObject spawnableObject = spawnManager.GetComponent<SpawnableObject>();
                    if (spawnableObject) spawnableObject.onDeactiveObject -= OnSpawnManagerDestroyed;

                    spawnManager.onFinishToSpawn -= OnFinishToSpawn;
                    spawnManager.onEverySpawn -= OnEverySpawn;
                }
            }
            spawnManagers.Clear();
        }

        /// <summary>
        /// When someone interact with this object
        /// </summary>
        /// <param name="whoInteract"></param>
        public void Interact(InteractComponent whoInteract)
        {
            //only if is open
            if (isOpen)
            {
                //save
                SaveClassLevelReached saveClass = SavesManager.instance && SavesManager.instance.Load<SaveClassLevelReached>() != null ? SavesManager.instance.Load<SaveClassLevelReached>() : new SaveClassLevelReached();
                if (saveClass.LevelReached < levelReach)
                {
                    saveClass.LevelReached = levelReach;
                    if (SavesManager.instance) SavesManager.instance.Save(saveClass);
                }

                //stop this script
                DeactiveExit();     //stop check open/close
                isOpen = false;     //can't interact anymore

                //call event
                onInteract?.Invoke(this);

                //change scene
                if (GameManager.instance && GameManager.instance.levelManager) 
                    GameManager.instance.levelManager.OnInteractExit(this);
            }
        }

        /// <summary>
        /// Call this when active a spawn manager that by default is deactivated. To add to the list of spawn managers to check
        /// </summary>
        /// <param name="spawnManager"></param>
        public void AddSpawnManager(SpawnManager spawnManager)
        {
            //if not already in the list
            if (spawnManager && spawnManagers.Contains(spawnManager) == false)
            {
                if (spawnManager.RestartWhenFinish == false)
                {
                    spawnManager.onFinishToSpawn += OnFinishToSpawn;
                    spawnManagers.Add(spawnManager);    //and add to the list
                }

                //add also every enemy spawned
                spawnManager.onEverySpawn += OnEverySpawn;
            }
        }

        #endregion

        #region events

        void OnEnemyDie(SpawnableObject enemy)
        {
            //when an enemy die, remove from the list
            if (enemies.Contains(enemy))
                enemies.Remove(enemy);

            //do checks
            DoCheck();
        }

        void OnPlayerChangeWeapon()
        {
            //when a player change weapon, do checks
            DoCheck();
        }

        void OnSpawnManagerDestroyed(SpawnableObject spawnableObject)
        {
            //when a spawn manager is destroyed
            if (spawnableObject)
            {
                SpawnManager spawnManager = spawnableObject.GetComponent<SpawnManager>();
                if (spawnManager)
                {
                    //remove spawn manager from the list
                    if (spawnManagers.Contains(spawnManager))
                        spawnManagers.Remove(spawnManager);
                }
            }

            //do checks
            DoCheck();
        }

        void OnFinishToSpawn(SpawnManager spawnManager)
        {
            //remove spawn manager from the list
            if (spawnManagers.Contains(spawnManager))
                spawnManagers.Remove(spawnManager);

            //do checks
            DoCheck();
        }

        void OnEverySpawn(GameObject spawnedObject)
        {
            if (spawnedObject == null)
                return;

            //if some spawn, spawn an enemy, register also to its enemy death
            Character character = spawnedObject.GetComponent<Character>();
            if (character && character.CharacterType == Character.ECharacterType.AI)
            {
                SpawnableObject spawnableObject = spawnedObject.GetComponent<SpawnableObject>();
                spawnableObject.onDeactiveObject += OnEnemyDie;
                enemies.Add(spawnableObject);   //and add to the list
            }

            //do checks
            DoCheck();
        }

        #endregion

        #region private API

        void DoCheck()
        {
            //check if can open, and if necessary change state
            if (isOpen != CheckCanOpen())
                ChangeExitState();
        }

        bool CheckCanOpen()
        {
            bool canOpen = true;

            //check there are not enemies in scene
            if (canOpen && checkNoEnemiesInScene)
            {
                canOpen = enemies.Count <= 0;
            }

            //check every player has weapon equipped
            if (canOpen && checkEveryPlayerHasWeapon)
            {
                foreach (Character player in players)
                {
                    //if someone has no weapon, can't open
                    if (player.GetSavedComponent<WeaponComponent>() == null || player.GetSavedComponent<WeaponComponent>().CurrentWeapon == null)
                    {
                        canOpen = false;
                        break;
                    }
                }
            }

            //check every spawn manager
            if (canOpen && checkEverySpawnManager)
            {
                canOpen = spawnManagers.Count <= 0;
            }

            return canOpen;
        }

        void ChangeExitState()
        {
            //open or close
            isOpen = !isOpen;

            //call event
            if (isOpen)
                onOpen?.Invoke();
            else
                onClose?.Invoke();
        }

        #endregion
    }
}