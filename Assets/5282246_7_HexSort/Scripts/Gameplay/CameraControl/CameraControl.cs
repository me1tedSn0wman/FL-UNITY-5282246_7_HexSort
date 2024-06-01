using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Control")]
  
    [SerializeField] private Transform focalPoint;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private Quaternion initRotation;

    public void Awake()
    {
        initRotation = focalPoint.rotation;
    }

    public void RotateCamera(float direction)
    {
        Vector3 rotation = direction * rotationSpeed * Vector3.up * Time.deltaTime;

        focalPoint.transform.Rotate(rotation);
    }

    public void ResetCameraRotation() {
        focalPoint.rotation = initRotation;
    }

    public void MoveFocalPointToPos(Vector3 newPos) {
        focalPoint.transform.position = newPos;
    }
}
