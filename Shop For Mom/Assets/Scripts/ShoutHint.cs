using UnityEngine;
using UnityEngine.UI;

public class ShoutUIHint : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject shoutArt;

    private CartSister sisterScript;

    void Start()
    {
        // Find the sister script in the scene
        sisterScript = Object.FindFirstObjectByType<CartSister>();
        
        // Hide the art at the start
        if (shoutArt != null) {
            shoutArt.SetActive(false);
        }
    }

    void Update()
    {
        if (sisterScript == null) {
            // Find sister
            sisterScript = Object.FindFirstObjectByType<CartSister>();
            return;
        }

        // Only show the art if she has left the cart and the player has shouts left
        bool showArt = sisterScript.leftCart && sisterScript.numShouts < 3;

        // Toggle the UI object
        if (shoutArt.activeSelf != showArt)
        {
            shoutArt.SetActive(showArt);
        }
    }
}