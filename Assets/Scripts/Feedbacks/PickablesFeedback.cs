using UnityEngine;
using redd096;
using redd096.GameTopDown2D;
using redd096.Attributes;

public class PickablesFeedback : MonoBehaviour
{
    [Header("Necessary Components - default get in parent")]
    [SerializeField] PickUpBASE pickable;

    [Header("On Pick")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnPick = default;
    [SerializeField] ParticleSystem particlesOnPick = default;
    [SerializeField] AudioClass audioOnPick = default;

    [Header("On Fail Pick (eg. full of health)")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnFailPick = default;
    [SerializeField] ParticleSystem particlesOnFailPick = default;
    [SerializeField] AudioClass audioOnFailPick = default;

    [Header("Camera Shake")]
    [SerializeField] bool cameraShakeOnPick = false;
    [SerializeField] bool cameraShakeOnFailPick = false;
    [EnableIf(EnableIfAttribute.EConditionOperator.OR, "cameraShakeOnPick", "cameraShakeOnFailPick")] [SerializeField] bool customShake = false;
    [EnableIf(EnableIfAttribute.EConditionOperator.OR, "cameraShakeOnPick", "cameraShakeOnFailPick")] [SerializeField] float shakeDuration = 1;
    [EnableIf(EnableIfAttribute.EConditionOperator.OR, "cameraShakeOnPick", "cameraShakeOnFailPick")] [SerializeField] float shakeAmount = 0.7f;

    void OnEnable()
    {
        //get references
        if (pickable == null) pickable = GetComponentInParent<PickUpBASE>();

        //add events
        if (pickable)
        {
            pickable.onPick += OnPick;
            pickable.onFailPick += OnFailPick;
        }
    }

    void OnDisable()
    {
        //remove events
        if (pickable)
        {
            pickable.onPick -= OnPick;
            pickable.onFailPick -= OnFailPick;
        }
    }

    void OnPick(PickUpBASE obj)
    {
        //instantiate vfx and sfx
        InstantiateGameObjectManager.instance.Play(gameObjectOnPick, transform.position, transform.rotation);
        ParticlesManager.instance.Play(particlesOnPick, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnPick, transform.position);

        //camera shake
        if (cameraShakeOnPick && CameraShake.instance)
        {
            //custom or default
            if (customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }

    void OnFailPick(PickUpBASE obj)
    {
        //instantiate vfx and sfx
        InstantiateGameObjectManager.instance.Play(gameObjectOnFailPick, transform.position, transform.rotation);
        ParticlesManager.instance.Play(particlesOnFailPick, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnFailPick, transform.position);

        //camera shake
        if (cameraShakeOnFailPick && CameraShake.instance)
        {
            //custom or default
            if (customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }
}
