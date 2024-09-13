using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LookAtFollower : MonoBehaviour
{
    public Transform looker;
    public bool lockXRotation = false;
    public bool lockYRotation = false;
    public bool lockZRotation = false;
    public bool flipLook = false;

    [HorizontalLine]

    public bool bob;
    [ShowIf("IsBobbing")] public float amplitude = 0.5f; // The height of the bob
    [ShowIf("IsBobbing")] public float speed = 1.0f; // The speed of the bobbing

    Vector3 startPosition;
    Transform target;

    void Start()
    {
        startPosition = transform.position;
        target = Camera.main.transform;
    }

    void Update()
    {
        Vector3 targetDirection = (flipLook ? looker.position - target.position : target.position - looker.position);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        if (lockXRotation || lockYRotation || lockZRotation)
        {
            Vector3 eulerRotation = targetRotation.eulerAngles;
            eulerRotation.x = lockXRotation ? looker.eulerAngles.x : eulerRotation.x;
            eulerRotation.y = lockYRotation ? looker.eulerAngles.y : eulerRotation.y;
            eulerRotation.z = lockZRotation ? looker.eulerAngles.z : eulerRotation.z;
            looker.rotation = Quaternion.Euler(eulerRotation);
        }
        else
            looker.rotation = targetRotation;

        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    bool IsBobbing() => bob;
}
