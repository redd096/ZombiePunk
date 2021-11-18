using UnityEngine;

public class Character : MonoBehaviour
{
    public enum ECharacterType { Player, AI};
    public ECharacterType CharacterType = ECharacterType.AI;
}
