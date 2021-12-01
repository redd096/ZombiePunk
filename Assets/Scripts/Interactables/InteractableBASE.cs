using UnityEngine;

namespace redd096
{
    public abstract class InteractableBASE : MonoBehaviour
    {
        /// <summary>
        /// When someone interact with this object
        /// </summary>
        /// <param name="whoInteract"></param>
        public abstract void Interact(Redd096Main whoInteract);
    }
}