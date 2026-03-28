using UnityEngine;

public class SpawningItems : MonoBehaviour
{
    public GameObject[] pickups;
    public GameObject[] spawners;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("SpawnPoints");
        int i = 0;
        foreach(GameObject spawn in spawners){
            pickups[i].transform.position = spawn.transform.position;
            pickups[i].SetActive(true);
            spawn.SetActive(false);
            Debug.Log(pickups[i].name + " moved to " + spawn.transform.position);

            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {


    }
}
