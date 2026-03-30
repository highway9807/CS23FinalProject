using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour
{
    // Score texts
    public TMP_Text totalScoreText;
    public TMP_Text correctItemsText;
    public TMP_Text incorrectItemsText;

    // Settings
    public int correctPoints = 100;
    public int incorrectPoints = 50;

    // These are private so other scripts can't accidentally 
    // mess them up without using our methods
    private int correctCount = 10;
    private int incorrectCount = 2;

    void Start()
    {
        // Calculate the total score
        int totalScore = correctCount*correctPoints - incorrectCount*incorrectPoints;
        // Update the text displays
        UpdateText(totalScoreText,
        $"Total Score: {correctCount} x {correctPoints} - {incorrectCount} x {incorrectPoints} = {totalScore}");
        UpdateText(correctItemsText, $"Correct Items: {correctCount}");
        UpdateText(incorrectItemsText, $"Incorrect Items: {incorrectCount}");
    }

    private void UpdateText(TMP_Text textObject, string message)
    {
        if (textObject != null) {
            textObject.text = message;
        }
        else {
            Debug.Log("Tried to update null text");
        }
    }
}