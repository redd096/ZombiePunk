using UnityEngine;
using NaughtyAttributes;

namespace redd096
{
    [AddComponentMenu("redd096/Components/Camera Component")]
    public class CameraComponent : MonoBehaviour
    {
        [Header("Camera - by default is MainCamera")]
        [SerializeField] Camera cameraToControl = default;

        [Header("Update camera position to follow this object")]
        [SerializeField] bool updatePosition = true;
        [EnableIf("updatePosition")] [SerializeField] Vector3 offsetPosition = new Vector3(0, 0, -10);

        [Header("Drop Camera On Death (necessary HealthComponent - default get from this gameObject)")]
        [SerializeField] bool dropCameraOnDeath = true;
        [EnableIf("dropCameraOnDeath")] [SerializeField] HealthComponent healthComponent = default;

        void OnEnable()
        {
            //get references
            if (cameraToControl == null) cameraToControl = Camera.main;
            if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();

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

        void Update()
        {
            //update camera position if necessary
            if (updatePosition && cameraToControl)
                cameraToControl.transform.position = transform.position + offsetPosition;
        }

        void OnDie(HealthComponent whoDied)
        {
            //drop camera on death, if setted
            if (dropCameraOnDeath && cameraToControl)
            {
                //only if child of this gameObject
                foreach(Camera childCam in GetComponentsInChildren<Camera>())
                {
                    if (childCam == cameraToControl)
                    {
                        cameraToControl.transform.SetParent(null);
                        break;
                    }
                }
            }
        }
    }
}