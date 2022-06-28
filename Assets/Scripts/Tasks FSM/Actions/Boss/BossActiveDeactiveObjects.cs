using UnityEngine;
using redd096;
using redd096.GameTopDown2D;

[AddComponentMenu("redd096/Tasks FSM/Action/Boss/Boss Active Deactive Objects")]
public class BossActiveDeactiveObjects : ActionTask
{
    [Header("Kill every summoned zombie")]
    [SerializeField] bool killSummonedZombies = true;

    [Header("Objects to activate")]
    [SerializeField] GameObject[] objectsToActivate = default;

    [Header("Objects to deactivate")]
    [SerializeField] GameObject[] objectsToDeactivate = default;

    [Header("Objects to destroy")]
    [SerializeField] GameObject[] objectsToDestroy = default;

    public override void OnEnterTask()
    {
        base.OnEnterTask();

        //kill every summoned zombie
        if (killSummonedZombies)
        {
            Character self = GetStateMachineComponent<Character>();

            foreach (Character character in FindObjectsOfType<Character>())
            {
                //find every AI in scene, not self, and kill it
                if (character.CharacterType == Character.ECharacterType.AI && character != self)
                {
                    character.GetSavedComponent<HealthComponent>().Die(self);
                }
            }
        }

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
