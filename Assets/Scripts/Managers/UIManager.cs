using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace redd096
{
    [AddComponentMenu("redd096/MonoBehaviours/UI Manager")]
    public class UIManager : MonoBehaviour
    {
        [Header("Menu")]
        [Min(0)] [SerializeField] float delayInputWhenOpenMenu = 0.3f;
        [SerializeField] GameObject pauseMenu = default;
        [SerializeField] GameObject endMenu = default;

        [Header("Game")]
        [SerializeField] Text ammoText = default;
        [SerializeField] Text currencyText = default;
        [SerializeField] string stringCurrency = "MONEY: ";

        [Header("Blood On Screen")]
        [SerializeField] Image[] bloodImages = default;
        [Tooltip("Value is alpha of the image")] [SerializeField] AnimationCurve curveDeactivationBlood = default;

        //delay input when open menu
        EventSystem eventSystem;
        Coroutine delayInputCoroutine;

        //blood on screen
        List<Image> deactiveBloods = new List<Image>();
        Dictionary<Image, Coroutine> activeBloods = new Dictionary<Image, Coroutine>();

        void Start()
        {
            //by default, deactive menus
            PauseMenu(false);
            EndMenu(false);

            //by default deactive blood images and add to list
            foreach (Image image in bloodImages)
            {
                if (image)
                {
                    image.gameObject.SetActive(false);
                    deactiveBloods.Add(image);
                }
            }
        }

        #region open menu with input delay

        public void OpenMenu(GameObject menu, bool active)
        {
            if (menu == null)
                return;

            //when active menu, deactive event system for a little time
            if (active && delayInputWhenOpenMenu > Mathf.Epsilon)
            {
                //remember event system
                if (EventSystem.current)
                {
                    eventSystem = EventSystem.current;
                    eventSystem.enabled = false;
                }

                //restart coroutine
                if (delayInputCoroutine != null) StopCoroutine(delayInputCoroutine);
                delayInputCoroutine = StartCoroutine(DelayInputCoroutine());
            }

            //active or deactive menu
            menu.SetActive(active);
        }

        IEnumerator DelayInputCoroutine()
        {
            //wait (real time, so if Time.timeScale is 0 it works anyway)
            float timeDelay = Time.realtimeSinceStartup + delayInputWhenOpenMenu;
            while (Time.realtimeSinceStartup < timeDelay)
                yield return null;

            //then re-enable event system
            if (eventSystem)
            {
                eventSystem.enabled = true;

                //try set singleton if not setted, because unity remove it when disabled
                if (EventSystem.current == null)
                    EventSystem.current = eventSystem;
            }
        }

        #endregion

        #region menu

        public void PauseMenu(bool active)
        {
            //active or deactive pause menu
            OpenMenu(pauseMenu, active);
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

            //active or deactive end menu
            OpenMenu(endMenu, active);
        }

        #endregion

        #region game

        /// <summary>
        /// Set current ammo text
        /// </summary>
        /// <param name="currentAmmo"></param>
        public void SetAmmoText(int currentAmmo)
        {
            if (ammoText)
                ammoText.text = currentAmmo.ToString();
        }

        /// <summary>
        /// Set currency text
        /// </summary>
        /// <param name="currentMoney"></param>
        public void SetCurrencyText(int currentMoney)
        {
            if (currencyText)
                currencyText.text = stringCurrency + currentMoney.ToString();
        }

        /// <summary>
        /// Show random blood images on screen
        /// </summary>
        public void ShowBloodOnScreen()
        {
            //if there aren't deactive bloods, take one random from active
            if(deactiveBloods.Count <= 0)
            {
                Image activeBlood = GetRandomBloodFromDictionary();
                if (activeBlood)
                {
                    //stop coroutine and add to deactive list
                    StopCoroutine(activeBloods[activeBlood]);
                    activeBloods.Remove(activeBlood);
                    deactiveBloods.Add(activeBlood);
                }
                else
                    return;
            }

            //active random blood
            Image randomBlood = deactiveBloods[Random.Range(0, deactiveBloods.Count)];
            randomBlood.gameObject.SetActive(true);

            //remove from list and add to dictionary (with coroutine for deactive)
            deactiveBloods.Remove(randomBlood);
            activeBloods.Add(randomBlood, StartCoroutine(DeactiveBloodOnScreen(randomBlood)));
        }

        #endregion

        #region private API

        Image GetRandomBloodFromDictionary()
        {
            //get random from dictionary
            int random = Random.Range(0, activeBloods.Keys.Count);
            int i = 0;
            foreach (Image blood in activeBloods.Keys)
            {
                i++;
                if (i == random)
                {
                    return blood;
                }
            }

            return null;
        }

        IEnumerator DeactiveBloodOnScreen(Image bloodOnScreen)
        {
            //until the end of curve
            float time = 0;
            while(time < curveDeactivationBlood.keys[curveDeactivationBlood.keys.Length -1].time)
            {
                //set blood alpha
                time += Time.deltaTime;
                bloodOnScreen.color = new Color(bloodOnScreen.color.r, bloodOnScreen.color.g, bloodOnScreen.color.b, curveDeactivationBlood.Evaluate(time));
                yield return null;
            }

            //deactive, remove from dictionary and add to the list deactive
            bloodOnScreen.gameObject.SetActive(false);
            activeBloods.Remove(bloodOnScreen);
            deactiveBloods.Add(bloodOnScreen);
        }

        #endregion
    }
}