using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasRotateField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool isPressed = false;

    [SerializeField] private float direction;

    [SerializeField] private CameraControl cameraControl;

    public void OnPointerDown(PointerEventData eventData) {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isPressed = false;
    }

    public void Update()
    {
        if (isPressed)
        {
            cameraControl.RotateCamera(direction);
        }
    }
}
