using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerItemPickup : MonoBehaviour
{
    public Transform player;
    public GameObject[] pickups;
    public GameObject[] spawners;
    private GameObject heldItem = null;

    void Start()
    {
        player = GetComponent<Transform>();
    }

    void Update()
    {
        // Find the closest pickup within range
        GameObject closest = null;
        float closestDist = 1.5f;
        foreach (GameObject pickup in pickups)
        {
            if (pickup == null) continue;
            float dist = Vector3.Distance(player.position, pickup.transform.position);
            if (dist <= closestDist)
            {
                closestDist = dist;
                closest = pickup;
                //
            }
        }

        // Color only the closest one blue, rest red
        foreach (GameObject pickup in pickups)
        {
            if (pickup == null) continue;
            Renderer renderer = pickup.GetComponent<Renderer>();
            if (pickup == closest)
            {
                Debug.Log(pickup.name + " is within the radius.");
                renderer.material.color = Color.blue;

            }
            else
            {
                renderer.material.color = Color.red;
            }
        }

        if (Input.GetKeyDown(KeyCode.P) && closest != null)
        {
            Debug.Log("Picked up " + closest.name);
            heldItem = closest;
            closest.SetActive(false);
        }

        GameObject closest_spawn = null;
        closestDist = 1.5f;
        foreach (GameObject spawn in spawners)
        {
            if (spawn == null) continue;
            float dist = Vector3.Distance(player.position, spawn.transform.position);
            if (dist <= closestDist)
            {
                closestDist = dist;
                closest_spawn = spawn;
            }
        }

 

        if (Input.GetKeyDown(KeyCode.F) && closest_spawn != null && heldItem != null)
        {
            Debug.Log("Dropped " + heldItem.name + " at " + closest_spawn.name);
            heldItem.transform.position = closest_spawn.transform.position;
            heldItem.SetActive(true);
            heldItem = null;
        }
    }
}
