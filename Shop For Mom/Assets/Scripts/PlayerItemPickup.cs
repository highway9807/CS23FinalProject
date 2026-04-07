using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerItemPickup : MonoBehaviour
{
    public Transform player;
    public GameObject[] pickups;
    public GameObject[] spawners;
    public AudioSource SFX_PickUp;
    private GameObject heldItem = null;
    private PlayerInventory playerInventory;

    void Start()
    {
        player = GetComponent<Transform>();
        pickups = GameObject.FindGameObjectsWithTag("Pickups");
        spawners = GameObject.FindGameObjectsWithTag("SpawnPoints");
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
            }
        }

        // Add glow to the closest pickup, remove glow from all others
        foreach (GameObject pickup in pickups)
        {
            if (pickup == null) continue;
            Transform glowChild = pickup.transform.Find("Glow");
            if (glowChild != null)
                glowChild.gameObject.SetActive(pickup == closest);
        }

        if (Input.GetKeyDown(KeyCode.P) && closest != null)
        {
            // Get the identity script from the object we are standing near
            ItemIdentity id = closest.GetComponent<ItemIdentity>();

            if (id != null && playerInventory != null)
            {
                // Try to add specific item type
                if (playerInventory.TryAdd(id.itemType)) {
                    Debug.Log("Picked up " + closest.name);
                    SFX_PickUp.Play();
                    heldItem = closest;
                    
                    closest.SetActive(false);
                    playerInventory.printInventory();
                }
                else {
                    Debug.Log($"There was an issue picking up an {id.itemType}");
                }
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
            Debug.Log($"playerInventory={playerInventory}, heldItem={heldItem}, closest_spawn={closest_spawn}");
        }

        if (Input.GetKeyDown(KeyCode.F) && closest_spawn != null && heldItem != null)
        {
            ItemIdentity heldId = heldItem.GetComponent<ItemIdentity>();
            if (playerInventory != null && heldId != null && playerInventory.TryRemove(heldId.itemType)) 
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
