using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ScoreHandler : MonoBehaviour
{
    // Score texts
    public TMP_Text scoreText;
    public TMP_Text messageText;
    public TMP_Text starsText;
    public TMP_Text correctText;
    public TMP_Text missingText;
    public TMP_Text incorrectText;
    public TMP_Text correctScoreText;
    public TMP_Text incorrectScoreText;
    public TMP_Text missingScoreText;
    public TMP_Text totalScoreText;


    // Settings
    private int correctPoints = 100;
    private int incorrectPoints = 75;
    private int missingPoints = 50;

    // shopping list and game handler
    public GameObject gameHandler;
    public ShoppingList shoppingList;


    // Private counters
    private int correctCount = 0;
    private int incorrectCount = 0;
    private int missingCount = 0;
    private int totalScore = 0;

    void Start() {
        // find the game handler
        gameHandler = GameObject.FindWithTag("GameHandler");
        shoppingList = FindObjectOfType<ShoppingList>();

        if (gameHandler == null) {
            Debug.LogError("Can't find game handler!");
            return;
        }

        // Calculate the total score
        calculateScore();

        // Display the updated score
        int totalScore = correctCount*correctPoints
            - incorrectCount*incorrectPoints
            - missingCount*missingPoints;
        
        // Update the text displays
        UpdateText(correctText, $"Correct items: {correctCount}");
        UpdateText(incorrectText, $"Incorrect items: {incorrectCount}");
        UpdateText(missingText, $"Missing items: {missingCount}");
        UpdateText(correctScoreText, $"+{correctCount * correctPoints}");
        UpdateText(incorrectScoreText, $"-{incorrectCount * incorrectPoints}");
        UpdateText(missingScoreText, $"-{missingCount * missingPoints}");
        UpdateText(scoreText, $"{totalScore}");
        
        if (totalScore >= GameHandler.gh.star3) {
            UpdateText(starsText, "3 Stars!");
            UpdateText(messageText, $"Congratuations! You've made your mom proud.");
        }
        else if (totalScore >= GameHandler.gh.star2) {
            UpdateText(starsText, "2 Stars!");
            UpdateText(messageText, $"Your mom could have done it better.");
        }
        else if (totalScore >= GameHandler.gh.star1) {
            UpdateText(starsText, "1 Star!");
            UpdateText(messageText, "Your mom says she's be better off without you. Prove her wrong!");
        }
        else {
            UpdateText(starsText, "0 Stars!");
            UpdateText(messageText, "I suggest you run away from home and never look back.");
        }

        if (gameHandler.GetComponent<GameHandler>().wasKickedOut) {
            UpdateText(messageText, "Your sister was caught by an employee and you were kicked out of the store!");
        }

        
        gameHandler.GetComponent<GameHandler>().setLevelScore(totalScore);
        int currScore = gameHandler.GetComponent<GameHandler>().getTotalScore();
        UpdateText(totalScoreText, $"Total Score: {currScore}");;
    }

    public void calculateScore() {
        // Debug.Log("Calculating score!");
        // Debug.Log($"shoppingList={shoppingList}, shopping_list={shoppingList?.shopping_list}");
        correctCount = 0;
        incorrectCount = 0;
        missingCount = 0;

        // Get the list of all items currently in the inventory
        PlayerInventory inventory = gameHandler.GetComponent<PlayerInventory>();

        ItemDefinition[] requiredItems = shoppingList?.shopping_list;
        
        if (requiredItems != null) {
            foreach (ItemDefinition listItem in requiredItems) {
                // Case 1: the item is in the shopping list, so it is correct
                if (inventory.hasItem(listItem)) {
                    correctCount++;
                    // Remove all instances correct items as we go
                    while (inventory.hasItem(listItem)) {
                        inventory.TryRemove(listItem);
                    }
                }
                // Case 2: the item should be here but is missing
                else {
                    missingCount++;
                }
            }
            // Case 3: the item is here and should not be
            incorrectCount += inventory.GetItemSlots().Count;
        }
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