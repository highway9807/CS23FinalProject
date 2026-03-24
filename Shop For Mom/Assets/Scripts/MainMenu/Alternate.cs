using UnityEngine;

public class Alternate : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public float switchTime = 1f;

    private bool isAActive = true;

    void Start()
    {
        InvokeRepeating("SwitchObjects", switchTime, switchTime);
    }

    void SwitchObjects()
    {
        isAActive = !isAActive;

        objectA.SetActive(isAActive);
        objectB.SetActive(!isAActive);
    }
}