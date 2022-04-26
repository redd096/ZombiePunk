using UnityEngine;

public class Fluctuate : MonoBehaviour
{
    [SerializeField] float height = 0.2f;

    Vector2 startPosition;

    void Start()
    {
        //save start position
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position = startPosition + Vector2.up * Mathf.Sin(Time.time) * height;
    }
}
