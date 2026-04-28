
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Pathfinding;
using TMPro;


public class CartSister : MonoBehaviour
{
    [Header("Dropping Settings")]
    public float minDropTime = 2f;
    public float maxDropTime = 5f;


    // The pickup prefab for spawing items
    public GameObject pickupPrefab;
    private CameraShake cameraShake;
   
    // The little sister needs access to the inventory to drop items and the
    // spawners to respawn them
    private PlayerInventory playerInventory;
    private SpawningItems spawner;


    public bool leftCart = false;
    // How far she can wander
    public float wanderRadius = 10f;


    // For AI naviagation
    public Transform target;
    IAstarAI ai;


    private GameObject player;
    private Animator anim;
    private SpriteRenderer sr;
    private GameObject[] messList;


    // The player only gets 3 shouts
    public int numShouts = 0;
    public TMP_Text shoutsText;


    // Sound effects
    public AudioSource throwGiggleSFX;
    public AudioSource pickupOohSFX;
    public AudioSource leaveCartGiggleSFX;
    public AudioSource callbackAwSFX;


    void Start()
    {
        player = GameObject.FindWithTag("Player");
        anim=GetComponentInChildren<Animator>();
        sr=GetComponentInChildren<SpriteRenderer>();
        playerInventory = GameHandler.gh.PlayerInventory;
        // Find the spawner script in the scene
        spawner = Object.FindFirstObjectByType<SpawningItems>();
        messList = GameObject.FindGameObjectsWithTag("Mess");
        cameraShake = Object.FindFirstObjectByType<CameraShake>();
       
        // The player starts with 3 shouts to use
        shoutsText.text = "Shouts Left: 3";


        // Find the AI component
        ai = GetComponent<IAstarAI>();
        ai.isStopped = true; // Since she starts in the cart, the AI is initially asleep
        anim.SetBool("Walk", false);


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
                throwGiggleSFX.Play();
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
        anim.SetTrigger("Pickup");
        Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();
        if (rb != null) {
            // Pick a random direction
            Vector2 randDir = Random.insideUnitCircle.normalized;
            // Apply the force so it slides away
            rb.AddForce(randDir * 1f, ForceMode2D.Impulse);
           
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
                    pickupOohSFX.Play();
                    anim.SetTrigger("Pickup");
                    closestObj.SetActive(false);
                    playerInventory.printInventory();
                }
                else {
                    // Debug.Log($"There was an issue picking up an {id.itemType}");
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
        // Don't leave if too close to the checkout
        if (getDistanceToClosestCheckout() < 5f) {
            // Debug.Log("Sister wanted to leave, but you were too close to checkout!");
            return;
        }
        // She is already wandering so we don't need to wander her again
        if (leftCart) {
            return;
        }
        leaveCartGiggleSFX.Play();
        leftCart = true;
        anim.SetBool("Walk", true);
        // She is no longer a child of player 1
        transform.SetParent(null);
       
        // Stop all other random actions
        StopAllCoroutines();
       
        GameObject closestMess = getClosestMess();


        if (closestMess != null) {
            // Debug.Log("The sister is going to: " + randomPoint);
            StartCoroutine(wander());
        }
        // Go back to the cart if she is close enough
        StartCoroutine(TryReturnToCart());
    }


//little sister wanders off
//little sister approaches a display
    IEnumerator wander() {
        anim.SetBool("Walk", true);
        // Find a point to wander to
        GameObject closestMess = getClosestMess();
        // Make sure the point is actually going somewhere
        if (closestMess != null) {
            // Set the destination
            ai.destination = closestMess.transform.position;
            // Find the path
            ai.SearchPath();


            // Wait until gets there
            while (!ai.reachedDestination || ai.pathPending) {
                yield return null;
            }


            // Stay at the new spot for a while before making a mess
            anim.SetBool("Walk", false);
            //add animation of her jumping up and down!
            anim.SetTrigger("Jump");
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            makeMess(closestMess);
            // The mess is already messed up so she shoundn't navigate to it again
            closestMess.tag = "DestroyedMess";
        }
        yield return null;
    }


    float getDistanceToClosestCheckout() {


        GameObject[] checkoutList = GameObject.FindGameObjectsWithTag("Checkout");
       
        // Return a very large number if none
        if (checkoutList == null || checkoutList.Length == 0) return Mathf.Infinity;


        float closestDistance = Mathf.Infinity;


        // Find the distance to the closest checkout
        foreach (GameObject currCheckout in checkoutList) {
            float currDistance = Vector3.Distance(transform.position, currCheckout.transform.position);
            if (currDistance < closestDistance) {
                closestDistance = currDistance;
            }
        }
        return closestDistance;
    }




    GameObject getClosestMess() {
        messList = GameObject.FindGameObjectsWithTag("Mess");
        // If there are no messes, none of them are closest
        if (messList == null || messList.Length == 0) {
            // Debug.Log("No messes found");
            return null;
        }
        // Find the closest mess
        GameObject closestMess = messList[0];
        foreach (GameObject currMess in messList) {
            float closestDistance = Vector3.Distance(transform.position, closestMess.transform.position);
            float currDistance = Vector3.Distance(transform.position, currMess.transform.position);


            if (currDistance < closestDistance) {
                closestMess = currMess;
            }
        }
        // Debug.Log("Sister going to closest mess: " + closestMess.transform.position);
        return closestMess;
    }


//Little sister attacks a display
    void makeMess(GameObject mess) {
        // Get the sprite renderers for the two sprites
        SpriteRenderer[] sprites = mess.GetComponentsInChildren<SpriteRenderer>(true);
        cameraShake.ShakeCamera(1f, 0.2f);
        sprites[0].gameObject.SetActive(false); // The first one is the tidy mess
        sprites[1].gameObject.SetActive(true); // The second one is the messed up mess
    }


    public void returnToCart() {
        // Only return if she has left
        if (!leftCart) {
            return;
        }
        // Keep track of the number of shouts
        numShouts++;
        callbackAwSFX.Play();
       
        // Stop the wandering loop
        StopAllCoroutines();
       
        // Start the journey back to the player
        StartCoroutine(ReturnToPlayerRoutine());
    }
   
    IEnumerator ReturnToPlayerRoutine() {
        // Enable AI so she can walk back
        ai.isStopped = false;
        anim.SetBool("Walk", true);
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
        anim.SetBool("Walk", false);


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
                    // Debug.Log("Player got close! Sister is hopping back in.");
                    returnToCart(); // Return to the cart
                    yield break;    // Stop this check since she's returning
                }
            }
            // Check 10 times a second
            yield return new WaitForSeconds(0.1f);
        }
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.J) && numShouts < 3 && leftCart) {
            numShouts++;
            shoutsText.text = "Shouts Left: " + (3 - numShouts);
            returnToCart();
        }
    }




   
}
   
   
