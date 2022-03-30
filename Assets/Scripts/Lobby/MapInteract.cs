using UnityEngine;

[System.Serializable]
public struct LevelStruct
{
    public GameObject ObjectToActivate;
    public int LevelToComplete;
}

public class MapInteract : BASELobbyInteract
{
    [Header("Levels to Unlock")]
    [SerializeField] LevelStruct[] levelsToUnlock = default;

    protected override void UpdateUI()
    {
        //load level reached
        SaveClassLevelReached saveClass = SavesManager.instance ? SavesManager.instance.Load<SaveClassLevelReached>() : null;
        int reachedLevel = saveClass != null ? saveClass.LevelReached : 0;

        //check every button if is unlocked
        foreach (LevelStruct level in levelsToUnlock)
        {
            if (level.ObjectToActivate)
                level.ObjectToActivate.SetActive(level.LevelToComplete <= reachedLevel);
        }
    }

    /// <summary>
    /// Delete saved Level Reached
    /// </summary>
    public void ClearLevelReached()
    {
        if (SavesManager.instance) SavesManager.instance.ClearSave<SaveClassLevelReached>();
        UpdateUI();
    }
}

