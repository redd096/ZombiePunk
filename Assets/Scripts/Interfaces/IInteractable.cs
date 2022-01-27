using UnityEngine;

namespace redd096
{
    public interface IInteractable
    {
        /// <summary>
        /// When someone interact with this object
        /// </summary>
        /// <param name="whoInteract"></param>
        void Interact(InteractComponent whoInteract);
    }
}