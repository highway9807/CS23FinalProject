using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Pathfinding;

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

    private bool leftCart = false;
    // How far she can wander
    public float wanderRadius = 10f;

    // For AI naviagation
    public Transform target;
    IAstarAI ai;

    private GameObject player;
    private Animator anim;
    private SpriteRenderer sr;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        anim=GetComponentInChildren<Animator>();
        sr=GetComponentInChildren<SpriteRenderer>();
        playerInventory = GameHandler.gh.PlayerInventory;
        // Find the spawner script in the scene
        spawner = Object.FindFirstObjectByType<SpawningItems>();

        // Find the AI component
        ai = GetComponent<IAstarAI>();
        ai.isStopped = true; // Since she starts in the cart, the AI is initially asleep

        // Start the random dropping
        StartCoroutine(RandomDrop());
        // Start the random pickups
        StartCoroutine(RandomPickup());
        // Start the random cart leaving
        StartCoroutine(RandomLeaveCart());
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

        if (closestObj == null) {
            return;
        }
        // Get the identity script from the object we are standing near
            ItemIdentity id = closestObj.GetComponent<ItemIdentity>();

            if (id != null && playerInventory != null)
            {
                // Try to add the closest item
                if (playerInventory.TryAdd(id.itemType)) {
                    Debug.Log("The little sister picked up " + closestObj.name + "!");
                    anim.SetTrigger("ispickingup");
                    closestObj.SetActive(false);
                    playerInventory.printInventory();
                }
                else {
                    Debug.Log($"There was an issue picking up an {id.itemType}");
                }
            }
    }

    IEnumerator RandomLeaveCart() {
        while (true) {
            // Generates the wait time
            float waitTime = Random.Range(minDropTime * 3, maxDropTime * 3);
            yield return new WaitForSeconds(waitTime);
            // Start the AI
            ai.isStopped = false;
            // Tries to drop an item
            TryLeaveCart();
        }
    }

    void TryLeaveCart() {
        Debug.Log("The sister is leaving the cart");
        // She is already wandering so we don't need to wander her again
        if (leftCart) {
            return;
        }
        leftCart = true;
        // She is no longer a child of player 1
        transform.SetParent(null);
        
        // Stop all other random actions
        StopAllCoroutines();
        
        Vector3 randomPoint = getWanderPoint(transform.position, wanderRadius);

        if (randomPoint != Vector3.zero) {
            // Debug.Log("The sister is going to: " + randomPoint);
            StartCoroutine(wander());
        }
        // Go back to the cart if she is close enough
        StartCoroutine(TryReturnToCart());
    }

    IEnumerator wander() {
        while (true) {
            // Find a point to wander to
            Vector3 randomPoint = getWanderPoint(transform.position, wanderRadius);
            // Make sure the point is actually going somewhere
            if (randomPoint != Vector3.zero) {
                // Set the destination
                ai.destination = randomPoint;
                // Find the path
                ai.SearchPath();

                // Wait until gets there
                while (!ai.reachedDestination || ai.pathPending) {
                    yield return null;
                }

                // Stay at the new spot for a while before going to the next one
                yield return new WaitForSeconds(Random.Range(2f, 5f));
            }
            yield return null;
        }
    }


    Vector3 getWanderPoint(Vector3 center, float radius) {
        // Gets a random point in the randius
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        // Gets the actual spot
        Vector3 targetPos = center + new Vector3(randomCircle.x, randomCircle.y, 0);
        
        // Find closest valid point on the Pathfinder grid
        var info = AstarPath.active.GetNearest(targetPos, NNConstraint.Default);
        // Make sure we can actually get to the point
        if (info.node != null && info.node.Walkable) {
            // return a Vector3 for movement
            return (Vector3)info.node.position;
        }
        // Return 0 by default so we can try again
        return Vector3.zero;
    }

    public void returnToCart() {
        // Only return if she has left
        if (!leftCart) {
            return;
        }
        Debug.Log("Little sister returning to the cart...");
        
        // Stop the wandering loop
        StopAllCoroutines(); 
        
        // Start the journey back to the player
        StartCoroutine(ReturnToPlayerRoutine());
    }
    
    IEnumerator ReturnToPlayerRoutine() {
        // Enable AI so she can walk back
        ai.isStopped = false;
        // Walk until she is close to the player
        while (Vector3.Distance(transform.position, player.transform.position) > 0.3f) {
            ai.destination = player.transform.position;
            yield return new WaitForSeconds(0.1f); 
        }

        // Become a child of the player again
        transform.SetParent(player.transform);

        // Wait for the end of frame to update her position
        yield return new WaitForEndOfFrame();
        
        // Go back into the cart
        transform.localPosition = new Vector3(1.446f, 0.343f, 0);
        transform.localRotation = Quaternion.identity;
        
        // Disable AI so she doesn't fight the player's movement
        ai.isStopped = true;
        leftCart = false;

        // Resume routines
        StartCoroutine(RandomDrop());
        StartCoroutine(RandomPickup());
        StartCoroutine(RandomLeaveCart());
    }

    IEnumerator TryReturnToCart() {
        // Cannot go back within 2 secs so that the sister has time to run a bit
        yield return new WaitForSeconds(2f); 
        // How close the player needs to be to pickup the sister
        float recallDistance = 2f; 

        while (leftCart) {
            if (player != null) {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                
                if (dist < recallDistance) {
                    Debug.Log("Player got close! Sister is hopping back in.");
                    returnToCart(); // Return to the cart
                    yield break;    // Stop this check since she's returning
                }
            }
            // Check 10 times a second
            yield return new WaitForSeconds(0.1f);
        }
    }

        void Update() {
            if (Input.GetKeyDown(KeyCode.J)) {
                Debug.Log("Sister returning to cart");
                returnToCart();
            }
        }
    }
    
    