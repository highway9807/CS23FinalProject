using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController_TopDown : MonoBehaviour {

    [Header("Movement settings")]
	private Animator anim;
    public float movementSpeed = 1.0f;
	private float movementBaseSpeed = 1f;
    public float acceleration = 5.0f; // How fast the player reaches max speed
    public float deacceleration = 3.0f; // How fast the player stops
    public float maxSpeed = 10;
    public bool movementEnabled = true;

    [Header("Puddle glide")]
    public float glideDuration = 1.25f;
    public float minGlideSpeed = 1f;
    public float maxGlideSpeed = 10f;

    [Header("Sprite Settings")]
    public Transform playerSpriteTransform;

    [Header("Sound effects")]
    public AudioSource playerWalk;
    public AudioSource wallThud;

    // Local Variables
    Vector2 inputVector;
    Rigidbody2D playerRb2D;
    float glideUntil;
    Vector2 glideVel;

    public void BeginGlide()
    {
        if (Time.time < glideUntil)
            return;
        Vector2 v = playerRb2D.linearVelocity;
        float m = v.magnitude;
        if (m < minGlideSpeed)
        {
            v = inputVector.normalized * movementSpeed;
            m = v.magnitude;
        }
        if (m < 0.001f)
        {
            float sx = playerSpriteTransform.localScale.x;
            v = new Vector2(sx >= 0f ? 1f : -1f, 0f) * minGlideSpeed;
            m = minGlideSpeed;
        }
        glideVel = v.normalized * Mathf.Clamp(m, minGlideSpeed, maxGlideSpeed);
        glideUntil = Time.time + glideDuration;
    }

    void Awake() {
        playerRb2D = GetComponent<Rigidbody2D>();
		anim = GetComponentInChildren<Animator>();
		movementBaseSpeed = movementSpeed;
    }

    void Update() {
		// Capture input (Dash needs a cooldown)
		if (Input.GetButtonDown("Dash")){
			movementSpeed *= 3f;
		}
		if (Input.GetButtonUp("Dash")){
			movementSpeed = movementBaseSpeed;
		}

        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        bool gliding = Time.time < glideUntil;

        // Keep the sprite from rotating if the parent object ever does
        if (playerSpriteTransform != null) {
            playerSpriteTransform.rotation = Quaternion.identity;
        }
        if (!gliding)
        {
            if (inputVector.x > 0) {
                playerSpriteTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (inputVector.x < 0) {
                playerSpriteTransform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            }

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
        else
        {
            anim.SetBool("Walk", true);
            if (!playerWalk.isPlaying)
                playerWalk.Play();
        }
    }

    void FixedUpdate() {
        ApplyMovement();
    }

    void ApplyMovement() {
        if (!movementEnabled)
        {
            glideUntil = 0f;
            playerRb2D.linearVelocity = Vector2.zero;
            return;
        }
        if (Time.time < glideUntil)
        {
            playerRb2D.linearVelocity = glideVel;
            return;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Person")) {
            
            GameHandler.gh.peopleHit++;
            StartCoroutine(showOuchArt(collision));
            Debug.Log("Hit person");
        }
    }

    IEnumerator showOuchArt(Collider2D personCollider) {
        GameObject ouchArt = personCollider.transform.Find("ouch_ART")?.gameObject;

        if (ouchArt != null) {
            Debug.Log("Showing ouch art");
            ouchArt.SetActive(true);

            yield return new WaitForSeconds(1f);

            ouchArt.SetActive(false);
            Debug.Log("Hiding ouch art");
        }
    }
}