using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class Employee : MonoBehaviour
{
    public GameObject littleSister;
    private bool isFollowing = false; // Controls if we are chasing the little sister
    
    // For ai navigation
    IAstarAI ai;

    void Start()
    {
        // Get the AI component
        ai = GetComponent<IAstarAI>();
        
        // Find the sister if not assigned in Inspector
        if (littleSister == null) {
            littleSister = GameObject.FindWithTag("Sister"); 
        }
    }

    void Update()
    {
        // If we aren't following yet, check if a mess was destroyed
        if (!isFollowing) {
            if (GameObject.FindWithTag("DestroyedMess") != null) {
                isFollowing = true;
                StartCoroutine(FollowSister());
            }
        }
    }

    IEnumerator FollowSister() {
        ai.isStopped = false;

        while (isFollowing) {
            if (littleSister != null && littleSister.GetComponent<CartSister>().leftCart) {
                // Constantly update the destination to her current position
                ai.destination = littleSister.transform.position;
                ai.SearchPath();

                // Check distance
                float dist = Vector3.Distance(transform.position, littleSister.transform.position);
                if (dist < 0.5f) {
                    GameHandler.gh.LoadScoreScene();
                }
            }
            else {
                isFollowing = false;
                ai.isStopped = true;
                ai.destination = transform.position;
            }
            
            // Wait for a short time so we don't calculate path every single frame
            yield return new WaitForSeconds(0.2f);
        }
    }
}