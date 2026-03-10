using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    // Code for animations and sound effects, we will use later

    //   private Animator animator;
    //   private bool FaceRight = false;
    //   public AudioSource WalkSFX;
      
      private Rigidbody2D rb2D;

    // Public movement/other settings
      public static float runSpeed = 10f;
      public float startSpeed = 10f;
      public bool isAlive = true;
    
    // Horizontal movement
      private Vector3 hMove;

      void Start(){
        
        // Again, we will use this later
        //    animator = gameObject.GetComponentInChildren<Animator>();

        // Get the rigid body
           rb2D = transform.GetComponent<Rigidbody2D>();
      }

      void Update() {
        // For now, isAlive is always true (I don't think our player dies)
        // We may add other states

            if (isAlive == true) {
                 // NOTE: Horizontal axis: [a] / left arrow is -1, [d] / right
                 // arrow is 1
                 // Get the horizontal movement from the keyboard input

                 // Apparently there was an update to keyboard input, so I had
                 // to add some code

                 // Get the current keyboard
                 var keyboard = Keyboard.current;
                 // Terminate if there is no keyboard
                if (keyboard == null) return;

                // Direct replacement for GetAxis
                float horizontal_input = 0;
                // Detect key presses and assign horizontal inputs
                if (keyboard.dKey.isPressed) horizontal_input = 1f;
                if (keyboard.aKey.isPressed) horizontal_input = -1f;
                // Create movement vector
                 hMove = new Vector3(horizontal_input, 0.0f, 0.0f);
           
                // More animation code we will use later
                //   if (Input.GetAxis("Horizontal") != 0){
                //         animator.SetBool ("Walk", true);
                //     //     if (!WalkSFX.isPlaying){
                //     //           WalkSFX.Play();
                //     //    }
                //   } else {
                //        animator.SetBool ("Walk", false);
                //     //    WalkSFX.Stop();
                //   }
           }
      }

      void FixedUpdate(){
            if (isAlive == true){
                  // Move the player:
                 rb2D.position = transform.position + hMove * runSpeed * Time.fixedDeltaTime;

                // We don't have turning quite yet

                //   // Turning: Reverse art if input is moving the Player right and Player faces left
                //  if ((hMove.x <0 && !FaceRight) || (hMove.x >0 && FaceRight)){
                //         playerTurn();
                //   }

                  //slow down on hills / stops sliding from velocity
                  if (hMove.x == 0){
                       rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x / 1.1f, rb2D.linearVelocity.y) ;
                  }
            }
      }

    // Once again, we don't have turning yet

    //   private void playerTurn(){
    //         // NOTE: Switch player facing label
    //         FaceRight = !FaceRight;

    //         // NOTE: Multiply player's x local scale by -1.
    //         Vector3 theScale = transform.localScale;
    //         theScale.x *= -1;
    //         transform.localScale = theScale;
    //   }
}
