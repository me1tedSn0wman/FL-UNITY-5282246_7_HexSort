using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DragState { 
    Empty,
    Dragging,
}

public class PlayerControlManager : MonoBehaviour
{
    [SerializeField] private DragState dragState;

    public float raycastDistance = 100f;

    [SerializeField] private Vector2 crntMousePosition;
    [SerializeField] public Vector3 crntMouseWordPosition;

    private Camera mainCamera;

    public void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    public void OnMousePosition(InputAction.CallbackContext context) {
        crntMousePosition = context.ReadValue<Vector2>();
        crntMouseWordPosition = GetPointerWordPosition(1);

        switch (dragState)
        {
            case DragState.Dragging:
                TryCheckCell();
                break;
            default:
                break;

        }
    }

    public void OnMouseButton(InputAction.CallbackContext context) {
        switch (context.phase) {
            case InputActionPhase.Started:
                TrySelectHexPieceTower();

                break;
            case InputActionPhase.Canceled:
                TryMoveTowerToCell();
                break;
        }
    }

    public void TrySelectHexPieceTower() {
        int layerMask = 1 << 7;

//        Debug.Log("Try select tower");

        RaycastHit hit;

        if (Physics.Raycast(crntMouseWordPosition, transform.TransformDirection(Vector3.forward), out hit, raycastDistance, layerMask)) {
            if (hit.transform.CompareTag("HexPieceTower")) {
                dragState = DragState.Dragging;
//                Debug.Log("Start Dragged: " + hit.transform.gameObject.name);

                HexPieceTower selectedTower = hit.transform.GetComponent<HexPieceTower>();
                if (selectedTower != null)
                {
                    GameplayManager.Instance.SetSelectedTower(selectedTower);
                }
                
            }
        }

    }

    public void TryMoveTowerToCell() {
        GameplayManager.Instance.ReleaseTower();
        dragState = DragState.Empty;
    }

    public void TryCheckCell() {
        int layerMask = 1 << 6;

        RaycastHit hit;

        if (Physics.Raycast(crntMouseWordPosition, transform.TransformDirection(Vector3.forward), out hit, raycastDistance, layerMask)) {
            if (hit.transform.CompareTag("HexCell")) { 
                
                HexCell selectedHexCell = hit.transform.GetComponent<HexCell>();
                if (selectedHexCell!= null)
                {
                    GameplayManager.Instance.SetSelectedCell(selectedHexCell);
                }
            }
        }
    }

    public Vector3 GetPointerWordPosition(float z) {
        Vector3 pointerPos = mainCamera.ScreenToWorldPoint( new Vector3 (
            crntMousePosition.x,
            crntMousePosition.y,
            z
            ));
        return pointerPos;
    }

    public Vector3 GetPointerWordPositionOnSurface() {
        int layerMask = 1 << 8;

        RaycastHit hit;

        if (Physics.Raycast(crntMouseWordPosition, transform.TransformDirection(Vector3.forward), out hit, raycastDistance, layerMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
