using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockWeapons : MonoBehaviour
{
    public float necessaryLevel;

    // Start is called before the first frame update
    void Start()
    {
        //load
        SaveClassLevelReached saveClass = SavesManager.instance ? SavesManager.instance.Load<SaveClassLevelReached>() : null;
        int levelReached = saveClass != null ? saveClass.LevelReached : 0;

        if (necessaryLevel <= levelReached)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
