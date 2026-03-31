using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerItemPickup : MonoBehaviour
{
    public Transform player;
    public GameObject[] pickups;
    public GameObject[] spawners;
    public ItemDefinition appleItem;
    public AudioSource audioSource;
    public AudioClip soundFX;
    private GameObject heldItem = null;
    private PlayerInventory playerInventory;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GetComponent<Transform>();
        pickups = GameObject.FindGameObjectsWithTag("Pickups");
        foreach (GameObject spawn in spawners){
            spawn.SetActive(false);
        }
        if (GameHandler.gh != null)
            playerInventory = GameHandler.gh.PlayerInventory;
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
                //Debug.Log(pickup.name + " is within the radius.");
                renderer.material.color = Color.blue;

            }
            else
            {
                renderer.material.color = Color.red;
            }
        }

        if (Input.GetKeyDown(KeyCode.P) && closest != null)
        {
            if (playerInventory != null && appleItem != null && playerInventory.TryAdd(appleItem))
            {
                Debug.Log("Picked up " + closest.name);
                audioSource.PlayOneShot(soundFX);
                heldItem = closest;
                closest.SetActive(false);
                playerInventory.printInventory();
            }
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

 
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"closest_spawn={closest_spawn}, heldItem={heldItem}");
        }

        if (Input.GetKeyDown(KeyCode.F) && closest_spawn != null && heldItem != null)
        {
            if (playerInventory != null && appleItem != null && playerInventory.TryRemove(appleItem))//include try Drop
            {
                Debug.Log("Dropped " + heldItem.name + " at " + closest_spawn.name);
                heldItem.transform.position = closest_spawn.transform.position;
                heldItem.SetActive(true);
                heldItem = null;
            }
        }
        pickups = GameObject.FindGameObjectsWithTag("Pickups");
    }
}
