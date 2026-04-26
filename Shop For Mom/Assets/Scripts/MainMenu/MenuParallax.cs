using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    //varaibles for parallax effect
    public float offsetMultiplier = 1f;
    public float smoothTime = .3f;
    private Vector2 startPosition;
    private Vector3 velocity;

    RectTransform rect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        rect = GetComponent<RectTransform>();
        //startPosition = rect.localPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //Parallax function
        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        transform.position = Vector3.SmoothDamp(transform.position, startPosition + (offset * offsetMultiplier), ref velocity, smoothTime);

    }

    void OnRectTransformDimensionChange()
    {
        if (rect != null)
        {
            rect.localPosition = startPosition;
        }
    }
}
