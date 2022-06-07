﻿using UnityEngine;
using UnityEngine.UI;
using redd096;
using redd096.Attributes;

public class OptionsManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Slider volumeMasterSlider = default;
    [SerializeField] Text volumeMasterText = default;
    [SerializeField] Slider volumeMusicSlider = default;
    [SerializeField] Text volumeMusicText = default;
    [SerializeField] Slider volumeSFXSlider = default;
    [SerializeField] Text volumeSFXText = default;
    [SerializeField] Toggle fullScreenToggle = default;
    [SerializeField] Toggle postProcessToggle = default;
    [SerializeField] Toggle dashToAimToggle = default;

    [Header("Default Values - set custom or take from bars and toggle in Editor")]
    [SerializeField] bool takeFromUI = true;
    [HideIf("takeFromUI")] [SerializeField] float volumeMasterDefault = 1;
    [HideIf("takeFromUI")] [SerializeField] float volumeMusicDefault = 1;
    [HideIf("takeFromUI")] [SerializeField] float volumeSFXDefault = 1;
    [HideIf("takeFromUI")] [SerializeField] bool fullScreenDefault = true;
    [HideIf("takeFromUI")] [SerializeField] bool postProcessEnabledDefault = true;
    [HideIf("takeFromUI")] [SerializeField] bool dashToAimDefault = true;

    [SerializeField] [ReadOnly] SaveClassOptions saveClassOptions;

    void Start()
    {
        //TODO
        //set in game post process e dash
        //
        //nel sound manager, quando si fa play with fade gli si passa un volume fisso.
        //Invece nella coroutine dovrebbe continuare a settare volume passato * volumeMusic o volumeSFX (ho creato una funzione che già riporta il settings in base all'audio source)

        //load options
        SaveClassOptions saveClass = SavesManager.instance ? SavesManager.instance.Load<SaveClassOptions>() : null;
        if (saveClass != null && saveClass.WasSaved)
        {
            saveClassOptions = saveClass;
        }
        //else create with default values (taken from UI or default values in inspector)
        else
        {
            saveClassOptions = new SaveClassOptions();
            saveClassOptions.WasSaved = true;   //set is created and saved. This is used to set default values instead of use only false and 0
            saveClassOptions.VolumeMaster = takeFromUI && volumeMasterSlider ? volumeMasterSlider.value : volumeMasterDefault;
            saveClassOptions.VolumeMusic = takeFromUI && volumeMusicSlider ? volumeMusicSlider.value : volumeMusicDefault;
            saveClassOptions.VolumeSFX = takeFromUI && volumeSFXSlider ? volumeSFXSlider.value : volumeSFXDefault;
            saveClassOptions.FullScreen = takeFromUI && fullScreenToggle ? fullScreenToggle.isOn : fullScreenDefault;
            saveClassOptions.PostProcessEnabled = takeFromUI && postProcessToggle ? postProcessToggle.isOn : postProcessEnabledDefault;
            saveClassOptions.DashToAimDirection = takeFromUI && dashToAimToggle ? dashToAimToggle.isOn : dashToAimDefault;

            //save
            if (SavesManager.instance) SavesManager.instance.Save(saveClassOptions);
        }

        //update UI
        UpdateUI();

        //set in game
        SetInGame();
    }

    void OnEnable()
    {
        //add events
        if (volumeMasterSlider) volumeMasterSlider.onValueChanged.AddListener(OnSetVolumeMaster);
        if (volumeMusicSlider) volumeMusicSlider.onValueChanged.AddListener(OnSetVolumeMusic);
        if (volumeSFXSlider) volumeSFXSlider.onValueChanged.AddListener(OnSetVolumeSFX);
        if (fullScreenToggle) fullScreenToggle.onValueChanged.AddListener(OnSetFullScreen);
        if (postProcessToggle) postProcessToggle.onValueChanged.AddListener(OnSetPostProcessEnabled);
        if (dashToAimToggle) dashToAimToggle.onValueChanged.AddListener(OnSetDashToAim);
    }

    void OnDisable()
    {
        //remove events
        if (volumeMasterSlider) volumeMasterSlider.onValueChanged.RemoveListener(OnSetVolumeMaster);
        if (volumeMusicSlider) volumeMusicSlider.onValueChanged.RemoveListener(OnSetVolumeMusic);
        if (volumeSFXSlider) volumeSFXSlider.onValueChanged.RemoveListener(OnSetVolumeSFX);
        if (fullScreenToggle) fullScreenToggle.onValueChanged.RemoveListener(OnSetFullScreen);
        if (postProcessToggle) postProcessToggle.onValueChanged.RemoveListener(OnSetPostProcessEnabled);
        if (dashToAimToggle) dashToAimToggle.onValueChanged.RemoveListener(OnSetDashToAim);
    }

    #region private API

    void SetInGame()
    {
        if (saveClassOptions != null)
        {
            AudioListener.volume = saveClassOptions.VolumeMaster;                                               //set volume master
            if (SoundManager.instance) SoundManager.instance.SetVolumeMusic(saveClassOptions.VolumeMusic);      //set volume music
            if (SoundManager.instance) SoundManager.instance.SetVolumeSFX(saveClassOptions.VolumeSFX);          //set volume sfx
            Screen.fullScreen = saveClassOptions.FullScreen;                                                    //set full screen
        }
    }

    void UpdateUI()
    {
        //update UI
        if (saveClassOptions != null)
        {
            //audio slider and text
            if (volumeMasterSlider) volumeMasterSlider.SetValueWithoutNotify(saveClassOptions.VolumeMaster);
            if (volumeMasterText) volumeMasterText.text = (saveClassOptions.VolumeMaster * 100).ToString("F0") + "%";
            if (volumeMusicSlider) volumeMusicSlider.SetValueWithoutNotify(saveClassOptions.VolumeMusic);
            if (volumeMusicText) volumeMusicText.text = (saveClassOptions.VolumeMusic * 100).ToString("F0") + "%";
            if (volumeSFXSlider) volumeSFXSlider.SetValueWithoutNotify(saveClassOptions.VolumeSFX);
            if (volumeSFXText) volumeSFXText.text = (saveClassOptions.VolumeSFX * 100).ToString("F0") + "%";

            //toggle
            if (fullScreenToggle) fullScreenToggle.SetIsOnWithoutNotify(saveClassOptions.FullScreen);
            if (postProcessToggle) postProcessToggle.SetIsOnWithoutNotify(saveClassOptions.PostProcessEnabled);
            if (dashToAimToggle) dashToAimToggle.SetIsOnWithoutNotify(saveClassOptions.DashToAimDirection);
        }
    }

    #endregion

    #region UI events

    public void OnSetVolumeMaster(float value)
    {
        //save
        if (saveClassOptions != null)
        {
            saveClassOptions.VolumeMaster = value;
            if (SavesManager.instance) SavesManager.instance.Save(saveClassOptions);
        }

        //update UI and set in game
        UpdateUI();
        SetInGame();
    }

    public void OnSetVolumeMusic(float value)
    {
        //save
        if (saveClassOptions != null)
        {
            saveClassOptions.VolumeMusic = value;
            if (SavesManager.instance) SavesManager.instance.Save(saveClassOptions);
        }

        //update UI and set in game
        UpdateUI();
        SetInGame();
    }

    public void OnSetVolumeSFX(float value)
    {
        //save
        if (saveClassOptions != null)
        {
            saveClassOptions.VolumeSFX = value;
            if (SavesManager.instance) SavesManager.instance.Save(saveClassOptions);
        }

        //update UI and set in game
        UpdateUI();
        SetInGame();
    }

    public void OnSetFullScreen(bool value)
    {
        //save
        if (saveClassOptions != null)
        {
            saveClassOptions.FullScreen = value;
            if (SavesManager.instance) SavesManager.instance.Save(saveClassOptions);
        }

        //update UI and set in game
        UpdateUI();
        SetInGame();
    }

    public void OnSetPostProcessEnabled(bool value)
    {
        //save
        if (saveClassOptions != null)
        {
            saveClassOptions.PostProcessEnabled = value;
            if (SavesManager.instance) SavesManager.instance.Save(saveClassOptions);
        }

        //update UI and set in game
        UpdateUI();
        SetInGame();
    }

    public void OnSetDashToAim(bool value)
    {
        //save
        if (saveClassOptions != null)
        {
            saveClassOptions.DashToAimDirection = value;
            if (SavesManager.instance) SavesManager.instance.Save(saveClassOptions);
        }

        //update UI and set in game
        UpdateUI();
        SetInGame();
    }

    #endregion
}
