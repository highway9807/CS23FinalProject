using UnityEngine;

public class SpawningItems : MonoBehaviour
{
    public GameObject[] pickups;
    public GameObject[] spawners;
    public ItemDefinition appleItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("SpawnPoints");
        int i = 0;
        foreach(GameObject spawn in spawners){
            if (i >= pickups.Length) break;

            pickups[i].transform.position = spawn.transform.position;
            pickups[i].SetActive(true);

            // Use ItemDefinition data for spawned world objects (apple for now).
            if (appleItem != null)
            {
                SpriteRenderer sr = pickups[i].GetComponent<SpriteRenderer>();
                if (sr != null && appleItem.sprite != null)
                    sr.sprite = appleItem.sprite;
                pickups[i].name = appleItem.itemName;
            }

            spawn.SetActive(false);
            Debug.Log(pickups[i].name + " moved to " + spawn.transform.position);

            i++;
        }
    }

    
    void Update()
    {


    }
}
