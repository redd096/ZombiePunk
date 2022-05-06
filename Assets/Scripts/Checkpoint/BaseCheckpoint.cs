using UnityEngine;

public class BaseCheckpoint : MonoBehaviour
{
    [Header("Objects to active and deactivate")]
    [SerializeField] GameObject[] objectsToActivate = default;
    [Space] 
    [SerializeField] GameObject[] objectsToDeactivate = default;

    [Header("Payload")]
    [SerializeField] GameObject payload = default;
    [SerializeField] Vector2 positionToSetOnLoad = Vector2.zero;

    public void LoadCheckpoint()
    {
        //active objects
        foreach (GameObject go in objectsToActivate)
            if (go)
                go.SetActive(true);

        //deactive objects
        foreach (GameObject go in objectsToDeactivate)
            if (go)
                go.SetActive(false);

        //set payload position
        if (payload)
            payload.transform.position = positionToSetOnLoad;
    }
}
