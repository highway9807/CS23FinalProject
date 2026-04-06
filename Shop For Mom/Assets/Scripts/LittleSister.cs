using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CartSister : MonoBehaviour
{
    [Header("Dropping Settings")]
    public float minDropTime = 15f;
    public float maxDropTime = 45f;
    
    // The little sister needs access to the inventory to drop items and the
    // spawners to respawn them
    private PlayerInventory playerInventory;
    private SpawningItems spawner;

    void Start()
    {
        playerInventory = GameHandler.gh.PlayerInventory;
        // Find the spawner script in the scene
        spawner = Object.FindFirstObjectByType<SpawningItems>();
        // Start the random dropping
        StartCoroutine(RandomDrop());
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
        Debug.Log("The little sister strikes!");
        // Get the items in the player's inventory
        List<ItemDefinition> currentItems = playerInventory.GetItemSlots();
        // Must check that there are items to be dropped
        if (currentItems.Count > 0) {
            // Pick a random item from the list to drp
            int randomIndex = Random.Range(0, currentItems.Count);
            ItemDefinition toDrop = currentItems[randomIndex];
            // Try to remove it from the player's inventory inventory
            if (playerInventory.TryRemove(toDrop)) {
                // Testing debug statement
                Debug.Log($"Little Sister tossed the {toDrop.itemName} out of the cart!");
                // Put the physical object back in the world at her feet
                SpawnItemAtFeet(toDrop);
            }
            else {
                Debug.Log($"Little Sister failed to toss the {toDrop.itemName}.");
            }
        }
        else {
            Debug.Log("Little sister tried to toss an item, but there were none.");
        }
    }

    void SpawnItemAtFeet(ItemDefinition item)
    {
        // Look through the global list of pickups
        foreach (GameObject p in spawner.pickups)
        {
            // Find a object that is currently hidden and matches
            if (p != null && !p.activeSelf)
            {
                // Check if the item matches what we need to drop
                if (p.name == item.itemName)
                {
                    // Drop it at the current location
                    p.transform.position = transform.position; 
                    p.SetActive(true);
                    break;
                }
            }
        }
    }
}