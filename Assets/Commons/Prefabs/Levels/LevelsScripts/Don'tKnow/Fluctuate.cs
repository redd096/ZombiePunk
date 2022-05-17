using UnityEngine;
using redd096.Attributes;

public class Fluctuate : MonoBehaviour
{
    [Header("Use animation curve or sin")]
    [SerializeField] bool useAnimationCurve = true;
    [EnableIf("useAnimationCurve")] [SerializeField] AnimationCurve animationCurve = default;
    [DisableIf("useAnimationCurve")] [SerializeField] float height = 0.5f;
    [DisableIf("useAnimationCurve")] [SerializeField] float speed = 1.5f;

    Vector2 startPosition;

    void Start()
    {
        //save start position
        startPosition = transform.position;
    }

    void Update()
    {
        if (useAnimationCurve)
            transform.position = startPosition + Vector2.up * animationCurve.Evaluate(Time.time);
        else
            transform.position = startPosition + Vector2.up * Mathf.Sin(Time.time * speed) * height;
    }
}
