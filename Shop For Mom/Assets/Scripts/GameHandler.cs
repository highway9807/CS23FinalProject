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
            startBtn.onClick.AddListener(onStartPressed);
        if(quitBtn != null) 
            quitBtn.onClick.AddListener(onQuitPressed);
        if(settingsBtn != null) 
            settingsBtn.onClick.AddListener(onSettingsPressed);
        if(credsBtn != null) 
            credsBtn.onClick.AddListener(onCredsPressed);
        if(how2PlayBtn != null) 
            credsBtn.onClick.AddListener(onHow2PlayPressed);
    }

    // click events
    private void onStartPressed()
    {
        if(startSceneName != null)
            SceneManager.LoadScene(startSceneName);
    }
    private void onCredsPressed()
    {
        if(credsSceneName != null)
            SceneManager.LoadScene(credsSceneName);
    }

    private void onHow2PlayPressed()
    {
        if(how2PlaySceneName != null)
            SceneManager.LoadScene(how2PlaySceneName);
    }


    private void onQuitPressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void onSettingsPressed()
    {
        
    }


    
}
