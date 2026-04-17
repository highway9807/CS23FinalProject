using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public Transform cameraTransform; 
    
    public float defaultDuration = 0.15f;
    public float defaultMagnitude = 0.3f;

    void Start() {
        // find the camera
        if (cameraTransform == null && transform.childCount > 0) {
            cameraTransform = GetComponentInChildren<Camera>().transform;
        }
    }

    public void ShakeCamera(float duration, float magnitude) {
        StartCoroutine(ShakeMe(duration, magnitude));
    }


    IEnumerator ShakeMe(float duration, float magnitude) {
        Vector3 startLocalPos = Vector3.zero; 
        float elapsed = 0.0f;

        while (elapsed < duration) {
            
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude ;
            
            // Apply to the camera trasnform
            if (cameraTransform != null) {
                cameraTransform.localPosition = new Vector3(x, y, startLocalPos.z);
            } else {
                // Fallback to this object if no child is assigned
                transform.localPosition = new Vector3(x, y, startLocalPos.z);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset postion to starting postiion
        if (cameraTransform != null) {
              cameraTransform.localPosition = startLocalPos;
       }
    }
}