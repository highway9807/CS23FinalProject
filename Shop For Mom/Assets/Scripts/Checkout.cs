using UnityEngine;

public class Checkout : MonoBehaviour
{

    private GameObject gameHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        gameHandler = GameObject.FindWithTag("GameHandler");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Navigate to the score scene when the player wants to checkout
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("Checking out...");
            gameHandler.GetComponent<GameHandler>().LoadScoreScene();
        }
    }
}
