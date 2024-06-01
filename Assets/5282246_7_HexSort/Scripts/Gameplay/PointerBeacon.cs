using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerBeacon : MonoBehaviour
{
    [SerializeField] private PlayerControlManager playerControlManager;

    public void Update() {
        transform.position = playerControlManager.crntMouseWordPosition;
    }
}
