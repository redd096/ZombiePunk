using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/Main/Character")]
    public class Character : Redd096Main
    {
        public enum ECharacterType { Player, AI };
        public ECharacterType CharacterType = ECharacterType.AI;
    }
}