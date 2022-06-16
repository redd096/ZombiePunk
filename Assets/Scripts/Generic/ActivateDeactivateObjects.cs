using UnityEngine;

public class ActivateDeactivateObjects : MonoBehaviour
{
    [Header("Objects to activate")]
    [SerializeField] GameObject[] objectsToActivate = default;

    [Header("Objects to deactivate")]
    [SerializeField] GameObject[] objectsToDeactivate = default;

    public void Activate()
    {
        //active objects
        foreach (GameObject go in objectsToActivate)
            if (go)
                go.SetActive(true);

        //deactive objects
        foreach (GameObject go in objectsToDeactivate)
            if (go)
                go.SetActive(false);
    }
}
