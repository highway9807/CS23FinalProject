using UnityEngine;
using TMPro;
using System.Collections.Generic; // I used this for Lists

public class ScoreHandler : MonoBehaviour
{
    // Score texts
    public TMP_Text totalScoreText;
    public TMP_Text correctItemsText;
    public TMP_Text incorrectItemsText;

    // Settings
    public int correctPoints = 100;
    public int incorrectPoints = 50;

    // Temporarily public
    public List<ItemDefinition> shoppingList = new List<ItemDefinition>();

    // Private counters
    private int correctCount = 0;
    private int incorrectCount = 0;
    private int totalScore = 0;

    void Start()
    {
        // Calculate the total score
        calculateScore();
        // Display the updated score
        int totalScore = correctCount*correctPoints - incorrectCount*incorrectPoints;
        // Update the text displays
        UpdateText(totalScoreText,
        $"Total Score: {correctCount} x {correctPoints} - {incorrectCount} x {incorrectPoints} = {totalScore}");
        if (totalScore > 100) UpdateText(totalScoreText, $"You Win!");
        else UpdateText(totalScoreText, $"You Lose!");
        UpdateText(correctItemsText, $"Correct Items: {correctCount}");
        UpdateText(incorrectItemsText, $"Incorrect Items: {incorrectCount}");
    }

    public void calculateScore() {
        correctCount = 0;
        incorrectCount = 0;

        // Get the list of all items currently in the inventory
        List<ItemDefinition> inventory = GameHandler.gh.PlayerInventory.GetItemSlots();

        // Loop through all items
        foreach (ItemDefinition inventoryItem in inventory) {
            bool isCorrect = false;
            // Check if the item is in the list
            foreach (ItemDefinition listItem in shoppingList) {
                if (inventoryItem == listItem) {
                    isCorrect = true;
                    correctCount++; // Count correct items
                    break;
                }
            }
            if (!isCorrect) {
                incorrectCount++; // Count incorrect items
            }
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