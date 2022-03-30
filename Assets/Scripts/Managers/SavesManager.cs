using System.Collections.Generic;
using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

//when add new classes, just create a new class (at the end of this file) and add in the array classesToSave

[DefaultExecutionOrder(-99)]
public class SavesManager : Singleton<SavesManager>
{
    //save and load in json
    ISaveClass[] classesToSave = new ISaveClass[3] { new SaveClassMoney(), new SaveClassBoughtWeapons(), new SaveLevelReached() };

    protected override void Awake()
    {
        base.Awake();

        //when start game, load files
        if (instance)
        {
            for (int i = 0; i < classesToSave.Length; i++)
            {
                //set only if file is not null (otherwise, keep empty class as setted in the array above)
                ISaveClass obj = SaveLoadJSON.Load(classesToSave[i].key, classesToSave[i].type) as ISaveClass;
                if (obj != null) classesToSave[i] = obj;
            }
        }
    }

    #region save and load json

    public void Save<T>(T classToSave)
    {
        for (int i = 0; i < classesToSave.Length; i++)
        {
            //find type in classes to save
            if (classesToSave[i].type == typeof(T))
            {
                //save in file and update variable
                SaveLoadJSON.Save(classesToSave[i].key, classToSave);
                classesToSave[i] = classToSave as ISaveClass;
                break;
            }
        }
    }

    public T Load<T>() where T : class
    {
        for (int i = 0; i < classesToSave.Length; i++)
        {
            //find type in classes to save
            if (classesToSave[i].type == typeof(T))
            {
                //return it (can't be null)
                return classesToSave[i] as T;
            }
        }

        return null;
    }

    public void ClearSave<T>()
    {
        for (int i = 0; i < classesToSave.Length; i++)
        {
            //find type in classes to save
            if (classesToSave[i].type == typeof(T))
            {
                //reset to an empty class (but never be null)
                SaveLoadJSON.DeleteData(classesToSave[i].key);
                classesToSave[i] = classesToSave[i].GetEmptyClass();
                break;
            }
        }
    }

    #endregion
}

#region interface

//interface to know where to save (key), type of the class and to reset generic class
public interface ISaveClass 
{ 
    string key { get; } 
    System.Type type { get; }

    ISaveClass GetEmptyClass();
}

#endregion

[System.Serializable]
public class SaveClassMoney : ISaveClass
{
    public string key => "Money";
    public System.Type type => typeof(SaveClassMoney);
    public ISaveClass GetEmptyClass() => new SaveClassMoney();

    //save money
    public int Money;
}

[System.Serializable]
public class SaveClassBoughtWeapons : ISaveClass
{
    public string key => "BoughtWeapons";
    public System.Type type => typeof(SaveClassBoughtWeapons);
    public ISaveClass GetEmptyClass() => new SaveClassBoughtWeapons();

    //save bought weapons
    public List<WeaponBASE> BoughtWeapons;
}

[System.Serializable]
public class SaveLevelReached : ISaveClass
{
    public string key => "LevelReached";
    public System.Type type => typeof(SaveLevelReached);
    public ISaveClass GetEmptyClass() => new SaveLevelReached();

    //save level reached
    public int LevelReached;
}
