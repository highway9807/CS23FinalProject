using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public static GameHandler gh;


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

    [SerializeField]
    private PlayerInventory playerInv;

    /// Inventory API for UI and gameplay systems. </summary>
    public PlayerInventory PlayerInventory => playerInv;

    void Awake()
    {
        // ensure no game object duplicates
        if ((gh != null) && (gh != this))
        {
            Destroy(gameObject);
            return;
        }

        // assign this particular obj to be the official gamestate
        gh = this;

        // allow gs to persist across scene changes
        DontDestroyOnLoad(gameObject);
        // create the 
        playerInv = GetComponent<PlayerInventory>();
        if(!playerInv)
        {
            playerInv = gameObject.AddComponent<PlayerInventory>();
        }
    }

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

    // Name: Update
    // Purpose: Provide quick keyboard pause toggle using Escape.
    // Inputs:  Escape key press.
    // Outputs: None.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();
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

    // Name: TogglePauseMenu
    // Purpose: Toggle pause state using PauseManager with PauseMenu reason.
    // Inputs:  None.
    // Outputs: None.
    public void TogglePauseMenu()
    {
        if (PauseManager.isPaused)
            PauseManager.ReleasePause(PauseReason.PauseMenu);
        else
            PauseManager.RequestPause(PauseReason.PauseMenu);
    }

    // Name: PauseFromButton
    // Purpose: Pause gameplay when called from a UI button.
    // Inputs:  None.
    // Outputs: None.
    public void PauseFromButton()
    {
        PauseManager.RequestPause(PauseReason.PauseMenu);
    }

    // Name: ResumeFromButton
    // Purpose: Resume gameplay when called from a UI button.
    // Inputs:  None.
    // Outputs: None.
    public void ResumeFromButton()
    {
        PauseManager.ReleasePause(PauseReason.PauseMenu);
    }


    
}
