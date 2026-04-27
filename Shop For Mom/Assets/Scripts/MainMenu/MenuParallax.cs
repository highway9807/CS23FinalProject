using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuParallax : MonoBehaviour
{
    //varaibles for parallax effect
    public float offsetMultiplier = 1f;
	float offsetStartMultiplyer;
    public float smoothTime = .3f;
    private Vector2 startPosition;
    private Vector3 velocity;

	//screen resize reset:
	private int lastWidth;
    private int lastHeight;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
		offsetStartMultiplyer = offsetMultiplier;
        
		//screen resize reset:
		lastWidth = Screen.width;
        lastHeight = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        //Parallax function
        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        transform.position = Vector3.SmoothDamp(transform.position, startPosition + (offset * offsetMultiplier), ref velocity, smoothTime);

		//screen resize reset:
		if (Screen.width != lastWidth || Screen.height != lastHeight) {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            OnScreenResized();
        }		
    }

//screen resize reset:
	void OnScreenResized() {
		startPosition = new Vector2 (Screen.width / 2,  Screen.height / 2);
		float resizeMuliplyer = (float)Screen.width / 1280f;
		offsetMultiplier = offsetStartMultiplyer * resizeMuliplyer;
		if (offsetMultiplier < offsetStartMultiplyer)
		{
			offsetMultiplier = offsetStartMultiplyer;
		}
		Debug.Log(" offsetMultiplyer = " + offsetMultiplier + ", " + offsetStartMultiplyer + ", " + Screen.width + " " + resizeMuliplyer);
    }

}
