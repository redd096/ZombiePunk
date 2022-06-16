using UnityEngine;

public class MagnetComponent : MonoBehaviour
{
    [SerializeField] GameObject coinmagnetPrefab = default;
    

    Transform magnet;

    void Start()
    {
        //instantiate prefab
        if (coinmagnetPrefab)
            magnet = Instantiate(coinmagnetPrefab, transform.position, Quaternion.identity).transform;
    }

    void Update()
    {
        //follow
        if (magnet)
            magnet.transform.position = transform.position;
    }
}
