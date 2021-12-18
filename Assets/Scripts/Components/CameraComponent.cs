using UnityEngine;
using NaughtyAttributes;

namespace redd096
{
    [AddComponentMenu("redd096/Components/Camera Component")]
    public class CameraComponent : MonoBehaviour
    {
        [Header("Camera - by default is MainCamera")]
        [SerializeField] Camera cameraToControl = default;

        [Header("Drop Camera On Death (necessary HealthComponent - default get from this gameObject)")]
        [SerializeField] bool dropCameraOnDeath = true;
        [EnableIf("dropCameraOnDeath")] [SerializeField] HealthComponent healthComponent = default;

        void Awake()
        {
            //get references
            cameraToControl = Camera.main;
        }

        void OnEnable()
        {
            //get references
            if (healthComponent == null)
                healthComponent = GetComponent<HealthComponent>();

            //add events
            if (healthComponent)
            {
                healthComponent.onDie += OnDie;
            }
        }

        void OnDisable()
        {
            //remove events
            if (healthComponent)
            {
                healthComponent.onDie -= OnDie;
            }
        }

        void OnDie(HealthComponent whoDied)
        {
            //drop camera on death, if setted
            if (dropCameraOnDeath && cameraToControl)
            {
                //only if child of this gameObject
                foreach(Camera childCam in GetComponentsInChildren<Camera>())
                {
                    if(childCam == cameraToControl)
                        cameraToControl.transform.SetParent(null);
                }
            }
        }
    }
}