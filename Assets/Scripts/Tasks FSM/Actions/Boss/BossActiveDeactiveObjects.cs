using UnityEngine;
using redd096;

[AddComponentMenu("redd096/Tasks FSM/Action/Boss/Boss Active Deactive Objects")]
public class BossActiveDeactiveObjects : ActionTask
{
    [Header("Objects to activate")]
    [SerializeField] GameObject[] objectsToActivate = default;

    [Header("Objects to deactivate")]
    [SerializeField] GameObject[] objectsToDeactivate = default;

    [Header("Objects to destroy")]
    [SerializeField] GameObject[] objectsToDestroy = default;

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //active
        foreach (GameObject go in objectsToActivate)
            if (go)
                go.SetActive(true);

        //deactive
        foreach (GameObject go in objectsToDeactivate)
            if (go)
                go.SetActive(true);

        //destroy
        foreach (GameObject go in objectsToDestroy)
            if (go)
                Destroy(go);
    }
}
