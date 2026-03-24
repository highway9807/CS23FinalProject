using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [Header("Game State")]
    // TODO: add player gameObject (pirvate, serialize)
    // TODO: add any TextMP objects (private, serialize)
    // TODO: initialize any gamestate variables we may need (private, serialize)
    [SerializeField]
    private string startSceneName;
    [SerializeField]
    private string credsSceneName;
    [SerializeField]
    private string how2PlaySceneName;

    [Header("Buttons")]
    [SerializeField]
    private Button startBtn; 
    [SerializeField]
    private Button quitBtn; 
    [SerializeField]
    private Button settingsBtn; 
    [SerializeField]
    private Button credsBtn;
    [SerializeField]
    private Button how2PlayBtn;
    //TODO: drag button prefabs 

    // add scene names to easily switch using scene manager (private, serialize)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // assign the click events
        if(startBtn != null) 
            startBtn.onClick.AddListener(loadStartScene);
        if(quitBtn != null) 
            quitBtn.onClick.AddListener(quitGame);
        if(settingsBtn != null) 
            settingsBtn.onClick.AddListener(loadSettingsScene);
        if(credsBtn != null) 
            credsBtn.onClick.AddListener(loadCredsScene);
        if(how2PlayBtn != null) 
            credsBtn.onClick.AddListener(loadH2PScene);
    }

    // click events
    private void loadStartScene()
    {
        if(startSceneName != null)
            SceneManager.LoadScene(startSceneName);
    }
    private void loadCredsScene()
    {
        if(credsSceneName != null)
            SceneManager.LoadScene(credsSceneName);
    }

    private void loadH2PScene()
    {
        if(how2PlaySceneName != null)
            SceneManager.LoadScene(how2PlaySceneName);
    }


    private void quitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void loadSettingsScene()
    {
        
    }


    
}
