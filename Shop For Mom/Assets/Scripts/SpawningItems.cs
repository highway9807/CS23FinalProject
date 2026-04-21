using UnityEngine;
using System.Collections.Generic;

public class SpawningItems : MonoBehaviour
{
    public GameObject[] prefabs;  // all possible item prefabs, each with ItemIdentity assigned
    public GameObject[] spawnPoints;

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");

        ShoppingList shoppingList = FindObjectOfType<ShoppingList>();

        // Shuffle spawn points so list items don't always appear in the same spots
        List<GameObject> shuffledSpawns = new List<GameObject>(spawnPoints);
        for (int i = shuffledSpawns.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffledSpawns[i], shuffledSpawns[j]) = (shuffledSpawns[j], shuffledSpawns[i]);
        }

        int spawnIndex = 0;

        // First: guarantee one spawn per shopping list item
        if (shoppingList != null)
        {
            foreach (ItemDefinition requiredItem in shoppingList.shopping_list)
            {
                if (spawnIndex >= shuffledSpawns.Count) break;

                GameObject matchingPrefab = FindPrefabForItem(requiredItem);
                if (matchingPrefab != null)
                {
                    SpawnItem(matchingPrefab, shuffledSpawns[spawnIndex].transform);
                    Debug.Log($"Spawned list item {requiredItem.itemName} at {shuffledSpawns[spawnIndex].transform.position}");
                    spawnIndex++;
                }
            }
        }

        // Fill remaining spawn points randomly
        while (spawnIndex < shuffledSpawns.Count)
        {
            int randIndex = Random.Range(0, prefabs.Length);
            SpawnItem(prefabs[randIndex], shuffledSpawns[spawnIndex].transform);
            Debug.Log($"Spawning random item at {shuffledSpawns[spawnIndex].transform.position}");
            spawnIndex++;
        }
    }

    GameObject FindPrefabForItem(ItemDefinition item)
    {
        foreach (GameObject prefab in prefabs)
        {
            ItemIdentity identity = prefab.GetComponent<ItemIdentity>();
            if (identity != null && identity.itemType == item)
                return prefab;
        }
        return null;
    }

    public void SpawnItem(GameObject prefab, Transform point)
    {
        Instantiate(prefab, point.position, point.rotation);
    }
}
