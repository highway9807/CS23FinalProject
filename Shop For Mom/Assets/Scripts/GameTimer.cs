using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    // The time remaining starts at 2 mins
    public float timeLeft = 120f;
    private bool timerStarted = false;

    // Timer display
    public TMP_Text timerText;
    public GameObject countdownArt;
    [SerializeField] private PlayerController_TopDown playerMovement;

    private GameObject three;
    private GameObject two;
    private GameObject one;

    private void Start() {

        DisplayTime(timeLeft);

        if (countdownArt != null) {
            // Find arts
            three = countdownArt.transform.Find("ThreeArt")?.gameObject;
            two = countdownArt.transform.Find("TwoArt")?.gameObject;
            one = countdownArt.transform.Find("OneArt")?.gameObject;

            // Hide them
            if (three) three.SetActive(false);
            if (two)   two.SetActive(false);
            if (one)   one.SetActive(false);
        }

        // Start the countdown
        if (three != null && two != null && one != null)
        {
            StartCoroutine(StartCountdown());
        }
    }

    IEnumerator StartCountdown()
    {
        playerMovement.movementEnabled = false;
        // Three
        three.SetActive(true);
        yield return new WaitForSeconds(1f);
        three.SetActive(false);

        // Two
        two.SetActive(true);
        yield return new WaitForSeconds(1f);
        two.SetActive(false);

        // One
        one.SetActive(true);
        yield return new WaitForSeconds(1f);
        one.SetActive(false);

        // Start the timer after the countdown ends
        timerStarted = true;
        playerMovement.movementEnabled = true;
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

