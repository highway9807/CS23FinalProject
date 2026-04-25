using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectFunction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SLevel0()
    {
        SceneManager.LoadScene("Level0");
    }

    public void SLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void SLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void SLevel3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void SLevel4()
    {
        SceneManager.LoadScene("Level4");
    }

    public void SLevel5()
    {
        SceneManager.LoadScene("Level5");
    }

    public void SLevel6()
    {
        SceneManager.LoadScene("Level6");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
