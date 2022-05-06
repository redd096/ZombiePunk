using UnityEngine;
using redd096.GameTopDown2D;

public class ReachedCheckpoint : MonoBehaviour
{
    [Header("Checkpoint")]
    [Min(1)] public int CheckpointNumber = 1;

    [Header("Rules")]
    [SerializeField] bool saveOnTriggerEnter = true;
    [SerializeField] bool saveOnTriggerExit = false;

    [Header("When save checkpoint, call load to activate/deactivate things?")]
    [SerializeField] bool loadOnSave = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (saveOnTriggerEnter == false)
            return;

        //try save checkpoint
        SaveCheckpoint(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (saveOnTriggerExit == false)
            return;

        //try save checkpoint
        SaveCheckpoint(collision);
    }

    void SaveCheckpoint(Collider2D collision)
    {
        //if hit player
        Character character = collision.GetComponentInParent<Character>();
        if (character && character.CharacterType == Character.ECharacterType.Player)
        {
            //save checkpoint
            if (SavesManager.instance)
            {
                SavesManager.instance.SaveCheckpoint(CheckpointNumber);

                //load if necessary
                if (loadOnSave)
                    LoadCheckpoint();
            }
        }
    }

    public void LoadCheckpoint()
    {
        //if there is attached a checkpoint script, load it
        BaseCheckpoint baseCheckpoint = GetComponent<BaseCheckpoint>();
        if (baseCheckpoint)
            baseCheckpoint.LoadCheckpoint();
    }
}
