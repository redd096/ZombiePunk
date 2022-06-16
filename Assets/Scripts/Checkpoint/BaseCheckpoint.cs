using UnityEngine;
using redd096.GameTopDown2D;
using redd096.Attributes;

public class BaseCheckpoint : MonoBehaviour
{
    [Header("Load character to this checkpoint position, or set custom position?")]
    [SerializeField] bool setCustomPosition = false;
    [EnableIf("setCustomPosition")] [SerializeField] Vector2 customPosition = Vector2.zero;

    [Header("Objects to active and deactivate")]
    [SerializeField] GameObject[] objectsToActivate = default;
    [Space] 
    [SerializeField] GameObject[] objectsToDeactivate = default;

    [Header("Payload")]
    [SerializeField] GameObject payload = default;
    [SerializeField] Vector2 positionToSetOnLoad = Vector2.zero;

    public void LoadCheckpoint()
    {
        //set player position (using custom or this checkpoint position)
        foreach (Character character in FindObjectsOfType<Character>())
            if (character.CharacterType == Character.ECharacterType.Player)
                character.transform.position = setCustomPosition ? customPosition : (Vector2)transform.position;

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
