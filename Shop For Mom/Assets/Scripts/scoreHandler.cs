using UnityEngine;
using TMPro;
using System.Collections.Generic; // I used this for Lists

public class ScoreHandler : MonoBehaviour
{
    // Score texts
    public TMP_Text totalScoreText;
    public TMP_Text messageText;
    public TMP_Text starsText;
    public TMP_Text correctText;
    public TMP_Text incorrectText;
    public TMP_Text correctScoreText;
    public TMP_Text incorrectScoreText;


    // Settings
    public int correctPoints = 100;
    public int incorrectPoints = 50;

    // shopping list and game handler
    public GameObject gameHandler;
    public ShoppingList shoppingList;


    // Private counters
    private int correctCount = 0;
    private int incorrectCount = 0;
    private int totalScore = 0;

    void Start() {
        // find the game handler
        gameHandler = GameObject.FindWithTag("GameHandler");
        if (gameHandler != null) {
            // get the shopping list
            shoppingList = FindObjectOfType<ShoppingList>();
        }
        else {
            Debug.LogError("Can't find game handler!");
            return;
        }

        // Calculate the total score
        calculateScore();

        // Display the updated score
        int totalScore = correctCount*correctPoints - incorrectCount*incorrectPoints;
        
        // Update the text displays
        UpdateText(correctText, $"Correct items: {correctCount}");
        UpdateText(incorrectText, $"Incorrect items: {incorrectCount}");
        UpdateText(correctScoreText, $"+{correctCount * correctPoints}");
        UpdateText(incorrectScoreText, $"-{incorrectCount * incorrectPoints}");
        UpdateText(totalScoreText, $"{totalScore}");
        
        if (totalScore > GameHandler.gh.star3) {
            UpdateText(starsText, "3 Stars!");
            UpdateText(messageText, $"Congratuations! You've made your mom proud.");
        }
        else if (totalScore > GameHandler.gh.star2) {
            UpdateText(starsText, "2 Stars!");
            UpdateText(messageText, $"Your mom could have done it better.");
        }
        else if (totalScore > GameHandler.gh.star1) {
            UpdateText(starsText, "1 Star!");
            UpdateText(messageText, "Your mom says she's be better off without you. Prove her wrong!");
        }
        else {
            UpdateText(starsText, "0 Stars!");
            UpdateText(messageText, "You lose! Try again.");
        }
    }

    public void calculateScore() {
        Debug.Log("Calculating score!");
        Debug.Log($"shoppingList={shoppingList}, shopping_list={shoppingList?.shopping_list}");
        correctCount = 0;
        incorrectCount = 0;

        // Get the list of all items currently in the inventory
        List<ItemDefinition> inventory = gameHandler.GetComponent<PlayerInventory>().GetItemSlots();

        // Loop through all items
        foreach (ItemDefinition inventoryItem in inventory) {
            bool isCorrect = false;
            // Check if the item is in the list
            foreach (ItemDefinition listItem in shoppingList.shopping_list) {
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