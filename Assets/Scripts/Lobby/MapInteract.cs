using redd096.GameTopDown2D;
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

    public override void Interact(InteractComponent whoInteract)
    {
        base.Interact(whoInteract);

        //save stats (to be sure everything is saved when leave lobby)
        if (SavesManager.instance && GameManager.instance && GameManager.instance.levelManager && GameManager.instance.levelManager.Players != null)
            SavesManager.instance.SaveStats(GameManager.instance.levelManager.Players.ToArray());
    }

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

    /// <summary>
    /// Save every level unlocked
    /// </summary>
    public void UnlockAllLevels()
    {
        //load level reached
        SaveClassLevelReached saveClass = SavesManager.instance && SavesManager.instance.Load<SaveClassLevelReached>() != null ? SavesManager.instance.Load<SaveClassLevelReached>() : new SaveClassLevelReached();
        int reachedLevel = saveClass.LevelReached;

        //find last possible level to reach
        foreach (LevelStruct level in levelsToUnlock)
        {
            if (level.LevelToComplete > reachedLevel)
                reachedLevel = level.LevelToComplete;
        }

        //save level reached
        saveClass.LevelReached = reachedLevel;
        if (SavesManager.instance) SavesManager.instance.Save(saveClass);

        //update UI
        UpdateUI();
    }
}

