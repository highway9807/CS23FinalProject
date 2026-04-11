using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CartSister : MonoBehaviour
{
    [Header("Dropping Settings")]
    public float minDropTime = 1f;
    public float maxDropTime = 5f;

    // The pickup prefab for spawing items
    public GameObject pickupPrefab;
    
    // The little sister needs access to the inventory to drop items and the
    // spawners to respawn them
    private PlayerInventory playerInventory;
    private SpawningItems spawner;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerInventory = GameHandler.gh.PlayerInventory;
        // Find the spawner script in the scene
        spawner = Object.FindFirstObjectByType<SpawningItems>();
        // Start the random dropping
        StartCoroutine(RandomDrop());
        // Start the random pickups
        StartCoroutine(RandomPickup());
    }

    // Waits a random amount of time (between the specified min and max),
    // and then drops an item
    IEnumerator RandomDrop() {
        while (true) {
            // Generates the wait time
            float waitTime = Random.Range(minDropTime, maxDropTime);
            yield return new WaitForSeconds(waitTime);
            // Tries to drop an item
            TryDropRandomItem();
        }
    }

    // Drops an item if there is one to drop
    void TryDropRandomItem() {
        //Debug.Log("The little sister strikes!");
        // Get the items in the player's inventory
        List<ItemDefinition> currentItems = playerInventory.GetItemSlots();
        // Must check that there are items to be dropped
        if (currentItems.Count > 0) {
            // Pick a random item from the list to drp
            int randomIndex = Random.Range(0, currentItems.Count);
            ItemDefinition toDrop = currentItems[randomIndex];
            // Try to remove it from the player's inventory inventory
            if (playerInventory.TryRemove(toDrop)) {
                // // Testing debug statement
                // Debug.Log($"Little Sister tossed the {toDrop.itemName} out of the cart!");
                // Put the physical object back in the world at her feet
                TossItem(toDrop);
            }
        }
    }

    void TossItem(ItemDefinition item) {
        // Create the object, it needs to be shrunk
        GameObject newObj = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
        newObj.transform.localScale = Vector3.one * 0.25f;
        // Set the sprite and data to be the ones in the definition given
        ItemIdentity id = newObj.GetComponent<ItemIdentity>();
        if (id != null) {
            id.itemType = item;
        }
        SpriteRenderer sr = newObj.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = item.sprite;

        // Actually toss the object
        Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();
        if (rb != null) {
            // Pick a random direction
            Vector2 randDir = Random.insideUnitCircle.normalized;
            // Apply the force so it slides away
            rb.AddForce(randDir * 10f, ForceMode2D.Impulse);
            
            // Add "Linear Drag" in the Inspector (around 5) so the item 
            // eventually slides to a stop instead of sliding forever!
        }
    }

    IEnumerator RandomPickup() {
        while (true) {
            // Generates the wait time
            float waitTime = Random.Range(minDropTime, maxDropTime);
            yield return new WaitForSeconds(waitTime);
            // Tries to drop an item
            TryPickup();
        }
    }

    void TryPickup() {
        PlayerItemPickup playerPickup = player.GetComponent<PlayerItemPickup>();
        GameObject closestObj = playerPickup.getClosest();
        // Get the identity script from the object we are standing near
            ItemIdentity id = closestObj.GetComponent<ItemIdentity>();

            if (id != null && playerInventory != null)
            {
                // Try to add the closest item
                if (playerInventory.TryAdd(id.itemType)) {
                    Debug.Log("Picked up " + closestObj.name);
                    closestObj.SetActive(false);
                    playerInventory.printInventory();
                }
                else {
                    Debug.Log($"There was an issue picking up an {id.itemType}");
                }
            }
    }
    
    }