using UnityEngine;

public class CanvasButtonFunctions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void CallBackSister() {
		// Find the little sister
		GameObject littleSis = GameObject.FindWithTag("LittleSister");
		if (littleSis == null) {
			Debug.Log("ERROR: Could not find sister to return to cart.");
			return;
		}
		// return her to the cart
		littleSis.GetComponent<CartSister>().returnToCart();
	}
}
