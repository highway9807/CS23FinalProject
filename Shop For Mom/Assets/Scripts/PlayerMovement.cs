using System.Collections.Generic;
using System.Collections;
using UnityEngine;

//This script goes onto the Player object
public class CarController_TopDown : MonoBehaviour {

    [Header("Car settings")]
    public float accelerationFactor = 10.0f;
    public float turnFactor = 3.5f;
    public float driftFactor = 0.95f; // adds more natural drift
    public float rotationLimiter = 5f; // increase to increase rotation angle.
    public bool allowStandingRotation = false; // enable for rotation without driving
    public float maxSpeed = 20; // Caps car speed

     // Local Variables
    Vector2 inputVector;
    float accelerationInput = 0f;
    float steeringInput = 0f;
    float rotationAngle = 0f;
    float velocityVsUp = 0f; // Track forwards velocity

    // Components
    Rigidbody2D carRb2D;

    void Awake(){
        carRb2D = GetComponent<Rigidbody2D>();
    }

    void Update(){
        inputVector = Vector2.zero;
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");
        SetInputVector(inputVector);
     }

    void FixedUpdate(){
          ApplyEngineForce();
          KillOrthogonalVelocity(); // Apply drifting function
          ApplySteering();
     }

     void ApplyEngineForce(){
          // Calculate directional velocity (how much "forward" we are moving)
          velocityVsUp = Vector2.Dot(transform.up, carRb2D.linearVelocity);

          // Limit velocity to maxSpeed
          if (velocityVsUp > maxSpeed && accelerationInput > 0) {
               return;
          }

          // Limit velocity reversing
          if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0) {
               return;
          }

          // Do the same for all other directions
          if (carRb2D.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0) {
               return;
          }

          // Apply drag if there is no acceleration input
          if (accelerationInput == 0) {
               carRb2D.linearDamping = Mathf.Lerp(carRb2D.linearDamping, 3.0f, Time.fixedDeltaTime * 3);
          } else {
               carRb2D.linearDamping = 0;
          }

          // Create a force for the car to start
          Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

          // Apply force to car
          carRb2D.AddForce(engineForceVector, ForceMode2D.Force);
     }

     void ApplySteering(){
          float minSpeedBeforeTurning = 1;
          // Limit the car's ability to turn when moving slowly:
          if (!allowStandingRotation){
               minSpeedBeforeTurning = (carRb2D.linearVelocity.magnitude / rotationLimiter);
               minSpeedBeforeTurning = Mathf.Clamp01(minSpeedBeforeTurning);
          }
          // Update rotation angle based on input direction
          rotationAngle -= steeringInput * turnFactor * minSpeedBeforeTurning;
          // Apply rotation to car
          carRb2D.MoveRotation(rotationAngle);         
     }

     void KillOrthogonalVelocity(){
          Vector2 fwdVelocity = transform.up * Vector2.Dot(carRb2D.linearVelocity, transform.up);
          Vector2 rightVelocity = transform.right * Vector2.Dot(carRb2D.linearVelocity, transform.right);
          carRb2D.linearVelocity = fwdVelocity + rightVelocity * driftFactor;         
     }

     void SetInputVector(Vector2 inputVector){
          steeringInput = inputVector.x;
          accelerationInput = inputVector.y;
     }
}