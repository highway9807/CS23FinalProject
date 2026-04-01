using UnityEngine;

public class SpawningItems : MonoBehaviour
{
    public GameObject[] pickups;
    public GameObject[] spawners;
    public ItemDefinition[] itemList;
    // public ItemDefinition appleItem;
    // public ItemDefinition bananaItem;
    // public ItemDefinition ketchupItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("SpawnPoints");
        int i = 0;
        foreach(GameObject spawn in spawners) {
            // Make sure we don't go past the end of either list
            if (i >= pickups.Length) break;
            if (i >= itemList.Length) break;

            pickups[i].transform.position = spawn.transform.position;
            pickups[i].SetActive(true);

            // Get the current item
            ItemDefinition currentItem = itemList[i];

            // Use ItemDefinition data for spawned world objects (apple for now).
            if (currentItem != null)
            {
                SpriteRenderer sr = pickups[i].GetComponent<SpriteRenderer>();
                if (sr != null && currentItem.sprite != null)
                    sr.sprite = currentItem.sprite;
                pickups[i].name = currentItem.itemName;
            }

            spawn.SetActive(false);
            // More specific debug statement
            Debug.Log($"Spawned {pickups[i].name} at {spawn.transform.position}");

            i++;
        }
    }

    
    void Update()
    {


    }
}
