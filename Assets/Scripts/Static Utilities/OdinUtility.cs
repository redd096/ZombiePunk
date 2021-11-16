using System.Collections.Generic;
using UnityEngine;

public static class OdinUtility
{
    public static List<T> GetListOfAssets<T>() where T : Object
    {
        List<T> list = new List<T>();

#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
        foreach (string guid in guids)
        {
            string pathToScript = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            list.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<T>(pathToScript));
        }
#endif

        return list;
    }
}
