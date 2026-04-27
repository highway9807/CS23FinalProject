using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShoppingList : MonoBehaviour
{
    [Header("Items required")]
    public ItemDefinition[] shopping_list;

    [Header("UI References")]
    public GameObject panel;          // the panel that shows/hides on L
    public Transform itemContainer;   // parent object holding the row prefabs
    public GameObject rowPrefab;      // a prefab with a TextMeshProUGUI component

    private PlayerInventory playerInventory;

    void Start()
    {
        if (FindObjectOfType<ShoppingList>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        panel = GameObject.FindWithTag("List");
        itemContainer = GameObject.Find("ItemContainer").transform;

        if (GameHandler.gh != null)
        {
            playerInventory = GameHandler.gh.PlayerInventory;
            playerInventory.Changed += RefreshUI;
        }

        panel.SetActive(true);
        RefreshUI();
    }

    void Update()
    {
        if (panel == null)
        {
            panel = GameObject.FindWithTag("List");
            GameObject container = GameObject.Find("ItemContainer");
            if (container != null) itemContainer = container.transform;

            // Re-subscribe to the new inventory when scene reloads
            if (GameHandler.gh != null && GameHandler.gh.PlayerInventory != playerInventory)
            {
                if (playerInventory != null) playerInventory.Changed -= RefreshUI;
                playerInventory = GameHandler.gh.PlayerInventory;
                playerInventory.Changed += RefreshUI;
            }

            if (panel != null) panel.SetActive(true);
        }
    }

    void RefreshUI()
    {
        if (itemContainer == null) return;

        // Clear old rows
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        // Build one row per shopping list item
        foreach (ItemDefinition item in shopping_list)
        {
            int have = playerInventory != null ? playerInventory.GetTotalItems(item) : 0;
            bool got  = have > 0;

            GameObject row = Instantiate(rowPrefab, itemContainer);
            TextMeshProUGUI label = row.GetComponent<TextMeshProUGUI>();

            label.text = (got ? "<s>" : "") + item.itemName + (got ? "</s>" : "");
			// first color = holding this item. second color = need this item:
            label.color = got ? Color.green : Color.black;
        }
    }

    void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.Changed -= RefreshUI;
    }
}
