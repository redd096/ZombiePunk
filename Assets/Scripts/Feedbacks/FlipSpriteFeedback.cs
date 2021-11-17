using UnityEngine;

public class FlipSpriteFeedback : MonoBehaviour
{
    [Header("Default get component in children")]
    [Tooltip("By default this sprite is looking to the right?")] [SerializeField] bool defaultLookRight = true;
    [SerializeField] SpriteRenderer spriteToFlip = default;

    AimComponent component;

    void Awake()
    {
        //get references
        component = GetComponent<AimComponent>();
        if (spriteToFlip == null) spriteToFlip = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        //add events
        if (component)
            component.onChangeAimDirection += OnChangeAimDirection;
    }

    void OnDisable()
    {
        //remove events
        if (component)
            component.onChangeAimDirection -= OnChangeAimDirection;
    }

    void OnChangeAimDirection(bool isLookingRight)
    {
        //flip right or left
        if (spriteToFlip)
            spriteToFlip.flipX = (defaultLookRight && isLookingRight == false) || (defaultLookRight == false && isLookingRight);
    }
}
