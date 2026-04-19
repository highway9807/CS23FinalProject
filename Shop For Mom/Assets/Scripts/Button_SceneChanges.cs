using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_SceneChanges : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ReturnToMainMenu()
    {
        GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>().LoadMainMenuScene();
    }

    public void LoadNextLevel()
    {
        GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>().LoadNextLevel();
    }
}
