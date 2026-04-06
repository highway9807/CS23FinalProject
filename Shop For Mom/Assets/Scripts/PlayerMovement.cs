using UnityEngine;

public class PlayerController_TopDown : MonoBehaviour {

    [Header("Movement settings")]
	private Animator anim;
    public float movementSpeed = 1.0f;
    public float acceleration = 5.0f; // How fast the player reaches max speed
    public float deacceleration = 3.0f; // How fast the player stops
    public float maxSpeed = 10; 

    [Header("Sprite Settings")]
    public Transform playerSpriteTransform;

    [Header("Sound effects")]
    public AudioSource playerWalk;
    public AudioSource wallThud;

    // Local Variables
    Vector2 inputVector;
    Rigidbody2D playerRb2D;

    void Awake() {
        playerRb2D = GetComponent<Rigidbody2D>();
		anim = GetComponentInChildren<Animator>();
    }

    void Update() {
        // Capture input
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        // Keep the sprite from rotating if the parent object ever does
        if (playerSpriteTransform != null) {
            playerSpriteTransform.rotation = Quaternion.identity;
        }
        if (inputVector.x > 0) {
            // Facing Right
            playerSpriteTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if (inputVector.x < 0) {
            // Facing Left
            playerSpriteTransform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }

		//walking audio
		if (inputVector.x !=0 || inputVector.y !=0)
		{
			anim.SetBool("Walk", true);
			if (!playerWalk.isPlaying){
			playerWalk.Play();
			}
		}
		else
		{
			anim.SetBool("Walk", false);
			playerWalk.Stop();
		}
    }

    void FixedUpdate() {
        ApplyMovement();
    }

    void ApplyMovement() {
        // Calculate targetVelocity
        Vector2 targetVelocity = inputVector.normalized * movementSpeed;
        // Calculate the difference between current and target velocity
        Vector2 velocityDiff = targetVelocity - playerRb2D.linearVelocity;
        // Determine if we are accelerating or braking
        float currentAcceleration = (inputVector.magnitude > 0) ? acceleration : deacceleration;
        // Apply a force to reach targetVelocity
        Vector2 movementForce = velocityDiff * currentAcceleration;
        playerRb2D.AddForce(movementForce);
        // cap spped at maxSpeed
        if (playerRb2D.linearVelocity.magnitude > maxSpeed) {
            playerRb2D.linearVelocity = playerRb2D.linearVelocity.normalized * maxSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Wall")) {
            // Only play thud if hitting reasonably hard
            if(collision.relativeVelocity.magnitude > 2f) {
                //playerAudio.PlayOneShot(wallThud);
				wallThud.Play();
            }
        }
    }
}