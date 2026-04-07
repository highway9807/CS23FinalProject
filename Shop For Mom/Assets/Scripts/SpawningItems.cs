using UnityEngine;


public class SpawningItems : MonoBehaviour
{
    public GameObject[] pickups;
    public GameObject[] prefabs;  // prefab with ItemIdentity set to apple asset
    public GameObject[] spawnPoints;
    //public ItemDefinition[] itemList;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        int i = 0;
        foreach(GameObject spawn in spawnPoints) {
            // Make sure we don't go past the end of either list

            //if (i >= pickups.Length) break;
            //if (i >= itemList.Length) break;

            //pickups[i].transform.position = spawn.transform.position;

            int rand_index = Random.Range(0, prefabs.Length);

            SpawnItem(prefabs[rand_index], spawn.transform);

            /*
            // Get the current item
            ItemDefinition currentItem = itemList[i];

            // Use ItemDefinition data for spawned world objects (apple for now).
            if (currentItem != null)
            {
                // 1. Get or add item identity slot if its missing
                ItemIdentity identity = pickups[i].GetComponent<ItemIdentity>();
                if (identity == null) {
                    identity = pickups[i].gameObject.AddComponent<ItemIdentity>();
                }

                // Assign the identity
                identity.itemType = currentItem;

                SpriteRenderer sr = pickups[i].GetComponent<SpriteRenderer>();
                if (sr != null && currentItem.sprite != null)
                    sr.sprite = currentItem.sprite;
                pickups[i].name = currentItem.itemName;
            }
            */

            //spawn.SetActive(false);
            // More specific debug statement
            //Debug.Log($"Spawned {pickups[i].name} at {spawn.transform.position}");
            Debug.Log($"Spawning at {spawn.transform.position}");
            spawn.SetActive(false);
            i++;
        }
        for (int j = 0; j < prefabs.Length; j++){
            prefabs[j].SetActive(false);
        }
    }

    
    void Update()
    {


    }
    public void SpawnItem(GameObject prefab, Transform point)
    {
        Instantiate(prefab, point.position, point.rotation);
    }
}
