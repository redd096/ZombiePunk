using UnityEngine;

public class MagnetComponent : MonoBehaviour
{
    [SerializeField] GameObject magnetPrefab = default;

    Transform magnet;

    void Start()
    {
        //instantiate prefab
        if (magnetPrefab)
            magnet = Instantiate(magnetPrefab, transform.position, Quaternion.identity).transform;
    }

    void Update()
    {
        //follow
        if (magnet)
            magnet.transform.position = transform.position;
    }
}
