using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnlockLevels : MonoBehaviour
{
    
    public GameObject levelPanel;
    Button[] levelButtons;
    int unlockIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unlockIndex = PlayerPrefs.GetInt("unlockIndex");
        levelButtons = new Button[levelPanel.transform.childCount];
        for (int i = 0; i< levelPanel.transform.childCount; i++)
        {
            levelButtons[i] = levelPanel.transform.GetChild(i).GetComponent<Button>();
        }

        for (int i = 0; i< levelButtons.Length; i++)
        {
            levelButtons[i].interactable = false;
        }

        for (int i = 0; i< unlockIndex +1; i++)
        {
            levelButtons[i].interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
