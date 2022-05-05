using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    enum ERotation { X, Y, Z }

    [SerializeField] ERotation rotationAxis = ERotation.Z;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(GetAxis() * speed * Time.deltaTime, Space.Self);
    }

    Vector3 GetAxis()
    {
        switch (rotationAxis)
        {
            case ERotation.X:
                return Vector3.right;
            case ERotation.Y:
                return Vector3.up;
            case ERotation.Z:
                return Vector3.forward;
            default:
                return Vector3.zero;
        }
    }
}
