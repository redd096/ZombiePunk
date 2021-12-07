using UnityEngine;
using UnityEngine.UI;

namespace redd096
{
    [AddComponentMenu("redd096/MonoBehaviours/UI Manager")]
    public class UIManager : MonoBehaviour
    {
        [Header("Menu")]
        [SerializeField] GameObject pauseMenu = default;
        [SerializeField] GameObject endMenu = default;

        [Header("Game")]
        [SerializeField] Text ammoText = default;

        void Start()
        {
            //by default, deactive menus
            PauseMenu(false);
            EndMenu(false);
        }

        #region menu

        public void PauseMenu(bool active)
        {
            if (pauseMenu == null)
            {
                return;
            }

            //active or deactive pause menu
            pauseMenu.SetActive(active);
        }

        public void EndMenu(bool active)
        {
            if (endMenu == null)
            {
                return;
            }

            //be sure to remove pause menu when active end menu
            if (active)
                PauseMenu(false);

            //active or deactive pause menu
            endMenu.SetActive(active);
        }

        #endregion

        #region ammo

        /// <summary>
        /// Set current ammo text
        /// </summary>
        /// <param name="currentAmmo"></param>
        public void SetAmmoText(int currentAmmo)
        {
            if (ammoText)
                ammoText.text = currentAmmo.ToString();
        }

        #endregion
    }
}