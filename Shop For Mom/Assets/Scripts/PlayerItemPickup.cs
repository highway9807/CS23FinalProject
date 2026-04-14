using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerItemPickup : MonoBehaviour
{
    public Transform player;
    public GameObject[] pickups;
    public GameObject[] spawners;
    public AudioSource SFX_PickUp;
    public SpawningItems spawner;
    private ItemDefinition heldItemDef = null;
    private PlayerInventory playerInventory;
    private GameObject closest = null;
    private Animator anim;
    public GameObject[] prefabs;


    void Start()
    {
        player = GetComponent<Transform>();
        pickups = GameObject.FindGameObjectsWithTag("Pickups");
        spawners = GameObject.FindGameObjectsWithTag("SpawnPoints");
        anim= GetComponentInChildren<Animator>();

        if (GameHandler.gh != null)
            playerInventory = GameHandler.gh.PlayerInventory;
    }

    // This gives the little sister access to the closest item so she can
    // randomly pick it up
    public GameObject getClosest() {
        return closest;
    }

    void Update()
    {
        // Find the closest pickup within range
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
                    if (anim != null)
                        anim.SetTrigger("pickup");

                    heldItemDef = id.itemType;
                    Destroy(closest);
                    closest = null;
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

        foreach (GameObject spawn in spawners)
        {
            if (spawn == null) continue;
            SpriteRenderer sr = spawn.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.enabled = (spawn == closest_spawn && heldItemDef != null);
        }



        
        if (Input.GetKeyDown(KeyCode.F) && closest_spawn != null && heldItemDef != null)
        {
            if (playerInventory != null && playerInventory.TryRemove(heldItemDef)) 
            {   
                foreach (GameObject prefab in prefabs)
                {
                    ItemIdentity prefabId = prefab.GetComponent<ItemIdentity>();
                    if (prefabId != null && prefabId.itemType == heldItemDef)
                    {
                        spawner.SpawnItem(prefab, closest_spawn.transform);
                        Debug.Log("Dropped " + heldItemDef.itemName + " at " + closest_spawn.name);
                        break;
                    }
                }
                heldItemDef = null;
            }
        }
        
        pickups = GameObject.FindGameObjectsWithTag("Pickups");
    }
}
