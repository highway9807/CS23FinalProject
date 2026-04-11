using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    public float offsetMultiplier = 1f;
    public float smoothTime = .3f;

    private Vector2 startPosition;
    private Vector3 velocity;

    private Vector3 Position;
    private bool prevScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        prevScreen = Screen.fullScreen;
        Reposition();
    }

    // Update is called once per frame
    void Update()
    {

        if (Screen.fullScreen != prevScreen)
        {
            prevScreen = Screen.fullScreen;
            Reposition();
        }

        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        transform.position = Vector3.SmoothDamp(transform.position, startPosition + (offset * offsetMultiplier), ref velocity, smoothTime);

    }

    void Reposition()
    {
        Position = transform.position;
    }
}
