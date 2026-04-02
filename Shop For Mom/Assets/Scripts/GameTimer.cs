using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    // The time remaining starts at 2 mins
    public float timeLeft = 120f;
    private bool timerStarted = false;

    // Timer display
    public TMP_Text timerText;

    private void Start()
    {
        // Start the timer when the scene begins
        timerStarted = true;
    }

    void Update()
    {
        if (timerStarted)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                DisplayTime(timeLeft);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeLeft = 0;
                timerStarted = false;
                
                // Redirect through the game handler
                GameHandler.gh.LoadScoreScene();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timerText == null) {
            return;
        }

        // Calculate minutes and seconds
        float mins = Mathf.FloorToInt(timeToDisplay / 60); 
        float secs = Mathf.FloorToInt(timeToDisplay % 60);

        // Format string
        timerText.text = string.Format("Time left: {0:00}:{1:00}", mins, secs);
    }
}