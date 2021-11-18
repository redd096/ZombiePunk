using UnityEngine;

public class FlipSpriteFeedback : MonoBehaviour
{
    [Header("Default get component in children")]
    [Tooltip("By default these sprites are looking to the right?")] [SerializeField] bool defaultLookRight = true;
    [SerializeField] SpriteRenderer[] spritesToFlip = default;

    AimComponent component;

    void OnEnable()
    {
        //get references
        component = GetComponent<AimComponent>();
        if (spritesToFlip == null || spritesToFlip.Length <= 0) spritesToFlip = GetComponentsInChildren<SpriteRenderer>();

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
        foreach(SpriteRenderer sprite in spritesToFlip)
            sprite.flipX = (defaultLookRight && isLookingRight == false) || (defaultLookRight == false && isLookingRight);
    }
}
