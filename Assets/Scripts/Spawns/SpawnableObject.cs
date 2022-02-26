using UnityEngine;

/// <summary>
/// This script will be added by Spawn.cs to every spawned object
/// </summary>
public class SpawnableObject : MonoBehaviour
{
    public System.Action<SpawnableObject> onDeactiveObject { get; set; }

    void OnDisable()
    {
        onDeactiveObject?.Invoke(this);
    }
}
