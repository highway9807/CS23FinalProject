using UnityEngine;

public class ButtonMoniter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] hints;
    private int index = 0;

    void Start()
    {
        for (int i = 0; i < hints.Length; i++)
        {
            if (i == 0) hints[i].SetActive(true);
            else hints[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (index >= hints.Length) return;

        if (index == 0 && Input.GetKeyDown(KeyCode.P)) Advance();

        else if (index == 1 && Input.GetKeyDown(KeyCode.F)) Advance();

        
    }

    void Advance()
    {
        hints[index].SetActive(false);
        index++;
        if (index < hints.Length)
        {
            hints[index].SetActive(true);
        }
    }
}
