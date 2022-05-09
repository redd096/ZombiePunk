using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using redd096.GameTopDown2D;

namespace redd096
{
    [AddComponentMenu("redd096/MonoBehaviours/UI Manager")]
    public class UIManager : MonoBehaviour
    {
        [Header("Menu")]
        [Min(0)] [SerializeField] float delayInputWhenOpenMenu = 0.3f;
        [SerializeField] GameObject pauseMenu = default;
        [SerializeField] GameObject endMenu = default;

        [Header("UI to disable when open Shop or others")]
        [SerializeField] GameObject[] uiToDisableWhenInShop = default;

        [Header("Ammo")]
        [SerializeField] Text ammoText = default;
        [SerializeField] Image bulletImage = default;
        [SerializeField] Sprite spriteWhenBulletIsNull = default;

        [Header("Currency")]
        [SerializeField] Text currencyText = default;
        [SerializeField] string stringCurrency = "MONEY: ";

        [Header("Perks")]
        [SerializeField] Image perkImage = default;
        [SerializeField] Image perkBackgroundImage = default;
        [SerializeField] bool fillPerkWithCooldown = true;
        [SerializeField] Sprite spriteWhenPerkIsNull = default;
        [SerializeField] Sprite backgroundSpriteWhenPerkIsNull = default;

        [Header("Blood On Screen")]
        [SerializeField] Image[] bloodImages = default;
        [Tooltip("Value is alpha of the image")] [SerializeField] AnimationCurve curveDeactivationBlood = default;

        [Header("Red Screen")]
        [SerializeField] Image redScreenImage = default;
        [Range(0.0f, 1.0f)] [SerializeField] float percentageToStartShowRedScreen = 0.4f;

        [Header("Flash On Get Damage or Health")]
        [SerializeField] Image imageToFlash = default;
        [SerializeField] bool useRealtimeTime = true;
        [SerializeField] Color colorOnGetDamage = Color.red;
        [SerializeField] Color colorOnGetHealth = Color.green;
        [SerializeField] float timeBeforeHideOnGetDamage = 0.1f;
        [SerializeField] float timeBeforeHideOnGetHealth = 0.1f;

        [Header("Combo Bars")]
        [SerializeField] Slider comboSlider = default;
        [SerializeField] bool hideDurationWhenNotActive = true;
        [SerializeField] Slider durationSuperWeaponSlider = default;

        //delay input when open menu
        EventSystem eventSystem;
        Coroutine delayInputCoroutine;

        Coroutine perkCooldownCoroutine;
        Coroutine flashImageCoroutine;

        //blood on screen
        List<Image> deactiveBloods = new List<Image>();
        Dictionary<Image, Coroutine> activeBloods = new Dictionary<Image, Coroutine>();

        void Start()
        {
            //by default, deactive menus
            PauseMenu(false);
            EndMenu(false);

            //deactive images
            UpdateRedScreenImage(0, 0);
            if (imageToFlash) imageToFlash.gameObject.SetActive(false);
            SetSuperWeaponIsActive(false);

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

        #region public API

        /// <summary>
        /// Disable UI when enter in shop or other menus. Or reactive it
        /// </summary>
        /// <param name="disable"></param>
        public void DisableUIWhenEnterInShop(bool disable)
        {
            foreach (GameObject go in uiToDisableWhenInShop)
                if (go)
                    go.SetActive(!disable);
        }

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
        /// Set bullet image
        /// </summary>
        /// <param name="sprite"></param>
        public void SetBulletImage(Sprite sprite)
        {
            if (bulletImage)
                bulletImage.sprite = sprite ? sprite : spriteWhenBulletIsNull;  //if sprite is null, use empty sprite
        }

        /// <summary>
        /// Set perk image and background image
        /// </summary>
        /// <param name="perk"></param>
        public void SetPerkImage(PerkData perk)
        {
            //set perk image
            if (perkImage)
            {
                //if perk is null, use empty sprite
                perkImage.sprite = perk && perk.PerkSprite ? perk.PerkSprite : spriteWhenPerkIsNull;

                //if sprite is null, deactive it
                perkImage.gameObject.SetActive(perkImage.sprite != null);
            }

            //set background image
            if (perkBackgroundImage)
            {
                //if perk is null, use empty sprite
                perkBackgroundImage.sprite = perk && perk.PerkBackgroundSprite ? perk.PerkBackgroundSprite : backgroundSpriteWhenPerkIsNull;

                //if sprite is null, deactive it
                perkBackgroundImage.gameObject.SetActive(perkBackgroundImage.sprite != null);
            }
        }

        /// <summary>
        /// Set cooldown on perk image
        /// </summary>
        /// <param name="perk"></param>
        public void SetPerkUsed(PerkData perk)
        {
            //start cooldown coroutine
            if (perkCooldownCoroutine != null)
                StopCoroutine(perkCooldownCoroutine);

            //only if setted
            if (fillPerkWithCooldown)
                perkCooldownCoroutine = StartCoroutine(PerkCooldownCoroutine(perk));
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

        /// <summary>
        /// Show ui feedback on get damage
        /// </summary>
        public void OnGetDamage(HealthComponent healthComponent)
        {
            //update red screen
            UpdateRedScreenImage(healthComponent.CurrentHealth, healthComponent.MaxHealth);

            //flash image
            if (flashImageCoroutine != null)
                StopCoroutine(flashImageCoroutine);

            flashImageCoroutine = StartCoroutine(FlashImageCoroutine(true));
        }

        /// <summary>
        /// Show ui feedback on get health
        /// </summary>
        public void OnGetHealth(HealthComponent healthComponent)
        {
            //update red screen
            UpdateRedScreenImage(healthComponent.CurrentHealth, healthComponent.MaxHealth);

            //flash image
            if (flashImageCoroutine != null)
                StopCoroutine(flashImageCoroutine);

            flashImageCoroutine = StartCoroutine(FlashImageCoroutine(false));
        }

        /// <summary>
        /// Set combo slider
        /// </summary>
        /// <param name="delta"></param>
        public void UpdateComboSlider(float delta)
        {
            if (comboSlider)
                comboSlider.value = delta;
        }

        /// <summary>
        /// Set super weapon slider
        /// </summary>
        /// <param name="delta"></param>
        public void UpdateSuperWeaponSlider(float delta)
        {
            if (durationSuperWeaponSlider)
                durationSuperWeaponSlider.value = delta;
        }

        /// <summary>
        /// Call this function to show/hide super weapon
        /// </summary>
        /// <param name="isActive"></param>
        public void SetSuperWeaponIsActive(bool isActive)
        {
            //if hide when not active, active/deactive slider
            if (hideDurationWhenNotActive && durationSuperWeaponSlider)
                durationSuperWeaponSlider.gameObject.SetActive(isActive);
        }

        #endregion

        #region private API

        IEnumerator PerkCooldownCoroutine(PerkData perk)
        {
            if (perkImage && perk)
            {
                //set fill animation
                while (perk && perk.GetPerkDeltaCooldown() < 1)
                {
                    perkImage.fillAmount = Mathf.Lerp(0, 1, perk.GetPerkDeltaCooldown());
                    yield return null;
                }
            }
        }

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

        private void UpdateRedScreenImage(float currentHealth, float maxHealth)
        {
            if (redScreenImage == null)
                return;

            //find percentage health
            float percentageHealth = currentHealth / maxHealth;

            //active only under percentage
            redScreenImage.gameObject.SetActive(percentageHealth <= percentageToStartShowRedScreen);

            //set alpha
            redScreenImage.color = new Color(redScreenImage.color.r, redScreenImage.color.g, redScreenImage.color.b, 1 - percentageHealth);
        }

        IEnumerator FlashImageCoroutine(bool getDamage)
        {
            //set color and show image
            if (imageToFlash)
            {
                imageToFlash.color = getDamage ? colorOnGetDamage : colorOnGetHealth;
                imageToFlash.gameObject.SetActive(true);
            }

            //wait realtime Time
            if (useRealtimeTime)
            {
                float time = Time.realtimeSinceStartup + (getDamage ? timeBeforeHideOnGetDamage : timeBeforeHideOnGetHealth);
                while (time > Time.realtimeSinceStartup)
                    yield return null;
            }
            //or Time.time
            else
            {
                yield return new WaitForSeconds(getDamage ? timeBeforeHideOnGetDamage : timeBeforeHideOnGetHealth);
            }

            //and hide
            if (imageToFlash)
                imageToFlash.gameObject.SetActive(false);
        }

        #endregion
    }
}