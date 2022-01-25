using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace redd096
{
    [AddComponentMenu("redd096/Interactables/Exit Interactable")]
    public class ExitInteractable : InteractableBASE
    {
        [Header("Rules to Open")]
        [Tooltip("Check there are no enemies in scene")] [SerializeField] bool checkNoEnemiesInScene = true;
        [Tooltip("If there are spawn managers in scene, check when every spawn has finished and every enemy is killed")] [SerializeField] bool checkEverySpawnManager = true;
        [Tooltip("Check every player has weapon")] [SerializeField] bool checkEveryPlayerHasWeapon = true;

        [Header("On Interact")]
        [SerializeField] [Scene] string sceneToLoad = default;

        [Header("DEBUG")]
        [ReadOnly] [ShowNonSerializedField] bool isOpen;
        public bool IsOpen => isOpen;

        [Button("ForceExit", EButtonEnableMode.Playmode)] public void ForceExit() { ChangeExitState(); }

        //events
        public System.Action onOpen { get; set; }
        public System.Action onClose { get; set; }
        public System.Action onInteract { get; set; }

        //necessary for checks
        List<Character> enemies = new List<Character>();
        List<Character> players = new List<Character>();
        List<SpawnManager> spawnManagers = new List<SpawnManager>();

        #region public API

        /// <summary>
        /// Level Manager call it to activate (become interactable and start check if can open)
        /// </summary>
        public void ActiveExit()
        {
            //register to every enemy death
            foreach (Character enemy in GameManager.instance.levelManager.Enemies)
            {
                if (enemy && enemy.GetSavedComponent<HealthComponent>())
                {
                    enemy.GetSavedComponent<HealthComponent>().onDie += OnEnemyDie;
                    enemies.Add(enemy);             //and add to the list
                }
            }

            //register to every player change weapon
            foreach (Character player in GameManager.instance.levelManager.Players)
            {
                if (player && player.GetSavedComponent<WeaponComponent>())
                {
                    player.GetSavedComponent<WeaponComponent>().onChangeWeapon += OnPlayerChangeWeapon;
                    players.Add(player);            //and add to the list
                }
            }

            //register to every spawn manager finish spawn
            foreach (SpawnManager spawnManager in GameManager.instance.levelManager.SpawnManagers)
            {
                if (spawnManager)
                {
                    spawnManager.onFinishToSpawn += OnFinishToSpawn;
                    spawnManagers.Add(spawnManager);    //and add to the list
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
            foreach (Character enemy in enemies)
            {
                if (enemy && enemy.GetSavedComponent<HealthComponent>())
                    enemy.GetSavedComponent<HealthComponent>().onDie -= OnEnemyDie;
            }
            enemies.Clear();

            //unregister from every player
            foreach (Character player in players)
            {
                if (player && player.GetSavedComponent<WeaponComponent>())
                    player.GetSavedComponent<WeaponComponent>().onChangeWeapon -= OnPlayerChangeWeapon;
            }
            players.Clear();

            //unregister from every spawm manager
            foreach (SpawnManager spawnManager in spawnManagers)
            {
                if (spawnManager)
                    spawnManager.onFinishToSpawn -= OnFinishToSpawn;
            }
            spawnManagers.Clear();
        }

        /// <summary>
        /// When someone interact with this object
        /// </summary>
        /// <param name="whoInteract"></param>
        public override void Interact(InteractComponent whoInteract)
        {
            //only if is open
            if (isOpen == false)
                return;

            //stop this script
            DeactiveExit();     //stop check open/close
            isOpen = false;     //can't interact anymore

            //call event
            onInteract?.Invoke();

            //change scene
            SceneLoader.instance.LoadScene(sceneToLoad);
        }

        #endregion

        #region events

        void OnEnemyDie(HealthComponent enemy)
        {
            //when an enemy died, remove from the list
            Character enemyCharacter = enemy.GetComponent<Character>();
            if (enemyCharacter)
            {
                if (enemies.Contains(enemyCharacter))
                    enemies.Remove(enemyCharacter);
            }

            //do checks
            DoCheck();
        }

        void OnPlayerChangeWeapon()
        {
            //when a player change weapon, do checks
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
                    if (player.GetSavedComponent<WeaponComponent>() == null || player.GetSavedComponent<WeaponComponent>().EquippedWeapon == null)
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