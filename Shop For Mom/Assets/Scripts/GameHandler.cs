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

    [Header("Main Menu Buttons")]
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

    [Header("In-game Buttons")]
    [SerializeField]
    private Button pauseInGame;

    [Header("Pause Menu Buttons")]
    [SerializeField]
    private GameObject pauseMenuPanel;
    [SerializeField]
    private Button resumeFromPause;
    [SerializeField]
    private Button quitFromPause;

    // add scene names to easily switch using scene manager (private, serialize)
    [Header("Inventory")]
    [SerializeField]
    private PlayerInventory playerInv;

    [Header("Level Management")]
    [SerializeField] private List<string> levelSceneNames = new List<string> {"Level0", "Level1", "Level2", "Level3", "Level4", "Level5", "Level6", "Level1_OLD"}; // List of scenes in order
    private int currentLevelIndex = 0; // The current scene we are on

    [Header("Stars")]
    public int star1, star2, star3;
    private List<List<int>> starPoints = new List<List<int>> {
        new List<int> {300, 200, 100},
        new List<int> {300, 200, 100},
        new List<int> {500, 300, 100},
        new List<int> {700, 400, 100},
        new List<int> {800, 400, 100},
        new List<int> {1000, 500, 100},
        new List<int> {1000, 500, 100},
        new List<int> {1000, 500, 100}
    };

    /// Inventory API for UI and gameplay systems.
    public PlayerInventory PlayerInventory => playerInv;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

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
        InitializePauseUiForScene(SceneManager.GetActiveScene());

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
            how2PlayBtn.onClick.AddListener(loadH2PScene);
        BindPauseButtons();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gh != this)
            return;
        // Find which level we are on
        int index = levelSceneNames.IndexOf(scene.name);
        if (index != -1) 
        {
            currentLevelIndex = index;
        }
        if (scene.name != "ScoreScene")
            playerInv.ClearAll();
        InitializePauseUiForScene(scene);
    }

    private void InitializePauseUiForScene(Scene scene)
    {
        RebindPauseReferences(scene);
        PauseManager.ClearAllPauses();
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        BindPauseButtons();
    }

    private void BindPauseButtons()
    {
        if (pauseInGame != null)
        {
            pauseInGame.onClick.RemoveListener(PauseFromButton);
            pauseInGame.onClick.AddListener(PauseFromButton);
        }
        if (resumeFromPause != null)
        {
            resumeFromPause.onClick.RemoveListener(ResumeFromButton);
            resumeFromPause.onClick.AddListener(ResumeFromButton);
        }
        if (quitFromPause != null)
        {
            quitFromPause.onClick.RemoveListener(LoadMainMenuScene);
            quitFromPause.onClick.AddListener(LoadMainMenuScene);
        }
    }

    private void RebindPauseReferences(Scene scene)
    {
        pauseMenuPanel = FindInSceneByName(scene, "PauseMenuPanel");
        pauseInGame = GetButtonByName(scene, "Button_PAUSE");
        resumeFromPause = GetButtonByName(scene, "ResumeButton");
        quitFromPause = GetButtonByName(scene, "QuitButton");
    }

    private Button GetButtonByName(Scene scene, string objectName)
    {
        GameObject go = FindInSceneByName(scene, objectName);
        if (go == null) return null;
        return go.GetComponent<Button>();
    }

    private GameObject FindInSceneByName(Scene scene, string objectName)
    {
        if (!scene.IsValid() || !scene.isLoaded) return null;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform match = FindChildByName(root.transform, objectName);
            if (match != null) return match.gameObject;
        }

        return null;
    }

    private Transform FindChildByName(Transform parent, string objectName)
    {
        if (parent.name == objectName) return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform match = FindChildByName(parent.GetChild(i), objectName);
            if (match != null) return match;
        }

        return null;
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

    // Name: PauseFromButton
    // Purpose: Pause gameplay when called from a UI button.
    // Inputs:  None.
    // Outputs: None.
    public void PauseFromButton()
    {
        PauseManager.RequestPause(PauseReason.PauseMenu);
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    // Name: ResumeFromButton
    // Purpose: Resume gameplay when called from a UI button.
    // Inputs:  None.
    // Outputs: None.
    public void ResumeFromButton()
    {
        PauseManager.ReleasePause(PauseReason.PauseMenu);
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    // Name: LoadScoreScene
    // Purpose: Load the score scene after the completion of a level
    // Inputs: None
    // Oututs: None
    public void LoadScoreScene()
    {
        star1 = starPoints[currentLevelIndex][2];
        star2 = starPoints[currentLevelIndex][1];
        star3 = starPoints[currentLevelIndex][0];
        // Find the timer and shut it down so it doesn't loop the scene
        GameTimer timer = GetComponentInChildren<GameTimer>();
        if (timer != null) {
            timer.enabled = false; 
        }
        
        PauseManager.ClearAllPauses();
        SceneManager.LoadScene("ScoreScene");
    }

    // Name: LoadMainMenu
    // Purpose: Load the main menu scene (from the score scee)
    // Inputs: None
    // Oututs: None
    public void LoadMainMenuScene() {
        PauseManager.ClearAllPauses();
        SceneManager.LoadScene("MainMenu");

        playerInv.ClearAll();
        currentLevelIndex = 0;
		// needs reset of all stats!!!
    }

    // Name: LoadNextLevel
    // Purpose: Load the next level (from the score scee)
    // Inputs: None
    // Oututs: None
    public void LoadNextLevel() 
    {
        currentLevelIndex++;

        if (currentLevelIndex < levelSceneNames.Count) 
        {
            PauseManager.ClearAllPauses();
            SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
        } 
        else 
        {
            // Send to the main menu when there are no more scenes
            Debug.Log("End of game reached!");
            LoadMainMenuScene();
        }
    }


    
}
