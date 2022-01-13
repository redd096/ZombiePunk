using UnityEngine;

public class NotifyCollisions : MonoBehaviour
{
    public System.Action<Collision2D> onCollisionEnter { get; set; }
    public System.Action<Collision2D> onCollisionStay { get; set; }
    public System.Action<Collision2D> onCollisionExit { get; set; }

    void OnCollisionEnter2D(Collision2D collision)
    {
        onCollisionEnter?.Invoke(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        onCollisionStay?.Invoke(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        onCollisionExit?.Invoke(collision);
    }
}
