using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FinalScoreHandler : MonoBehaviour
{

    // Score texts
    public TMP_Text scoreText;
    public TMP_Text level0Text;
    public TMP_Text level1Text;
    public TMP_Text level2Text;
    public TMP_Text level3Text;
    public TMP_Text level4Text;
    public TMP_Text level5Text;
    public TMP_Text level6Text;
    public TMP_Text messageText;

    private List<int> finalLevelScores;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finalLevelScores = GameHandler.gh.GetComponent<GameHandler>().levelScores;
        updateScores();
    }

    // Update is called once per frame
    void updateScores()
    {
        int totalScore = GameHandler.gh.GetComponent<GameHandler>().getTotalScore();
        scoreText.text = $"Total Score: {totalScore}";

        if (totalScore >= 2800) {
            messageText.text = "Congratuations! You've made your mom proud.";
        }
        else if (totalScore >= 1500) {
            messageText.text = "Your mom could have done it better.";
        }
        if (totalScore >= 0) {
            messageText.text = "Your mom says she's be better off without you.";
        }
        else {
            messageText.text = "You lose!";
        }

        level0Text.text = $"Level 0: {finalLevelScores[0]}";
        level1Text.text = $"Level 1: {finalLevelScores[1]}";
        level2Text.text = $"Level 2: {finalLevelScores[2]}";
        level3Text.text = $"Level 3: {finalLevelScores[3]}";
        level4Text.text = $"Level 4: {finalLevelScores[4]}";
        level5Text.text = $"Level 5: {finalLevelScores[5]}";
        level6Text.text = $"Level 6: {finalLevelScores[6]}";
    }
}
