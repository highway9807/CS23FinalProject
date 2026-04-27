using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    //varaibles for parallax effect
    public float offsetMultiplier = 1f;
    public float smoothTime = .3f;
    private Vector2 startPosition;
    private Vector3 velocity;


	Vector3 originalPos;
    RectTransform rect;
	Vector2 lastSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        rect = GetComponent<RectTransform>();
		originalPos = rect.localPosition;
		lastSize = rect.rect.size;
        //startPosition = rect.localPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Parallax function
        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        transform.position = Vector3.SmoothDamp(transform.position, startPosition + (offset * offsetMultiplier), ref velocity, smoothTime);

    }

	//Called automatically on screen resize
    void OnRectTransformDimensionsChange()
    {
		Debug.Log("current rect size: " + rect.rect.size);
        if ((rect != null) && (rect.rect.size != lastSize))
        {
			Debug.Log("current rect size: " + rect.rect.size);
            rect.localPosition = originalPos;
			lastSize = rect.rect.size;
			//rectTransform.rect.size;
        }
    }
}
