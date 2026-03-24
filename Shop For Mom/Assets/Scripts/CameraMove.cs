using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    
    public Transform player;
    private Vector3 offset = new Vector3(0, 0, -1f);
    
    // Update is called once per frame
    void LateUpdate()
    {
        // Terminate if there is no player
        if (player == null) return;
        // Calculate the desired postion
        Vector3 desiredPosition = player.position + offset;
        // Move there smoothly
        transform.position = desiredPosition;
    }
}
