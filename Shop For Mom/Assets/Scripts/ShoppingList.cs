using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShoppingList : MonoBehaviour
{
    [Header("Items required")]
    public ItemDefinition[] shopping_list;

    public bool IsOnList(ItemDefinition item)
    {
        if (item == null || shopping_list == null) return false;
        for (int i = 0; i < shopping_list.Length; i++)
            if (shopping_list[i] == item) return true;
        return false;
    }

    [Header("UI References")]
    public GameObject panel;
    public Transform itemContainer;
    public GameObject rowPrefab;
    public float rowHeight = 30f;
    public float verticalOffset = 20f;

    private PlayerInventory playerInventory;

    void Start()
    {
        if (FindObjectOfType<ShoppingList>() != this && FindObjectOfType<ShoppingList>() != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        FindUIReferences();
    }

    void Update()
    {
        // Find UI references if the container was destroyed
        if (itemContainer == null || !itemContainer.gameObject.scene.IsValid()) {
            FindUIReferences();
        }
    }

    void FindUIReferences()
    {
        panel = GameObject.FindWithTag("List");
        GameObject container = GameObject.Find("ItemContainer");

        if (container != null)
        {
            itemContainer = container.transform;
            
            if (GameHandler.gh != null)
            {
                if (playerInventory != null) playerInventory.Changed -= RefreshUI;
                playerInventory = GameHandler.gh.PlayerInventory;
                playerInventory.Changed += RefreshUI;
            }

            if (panel != null) panel.SetActive(true);
            
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        // If we don't have the container or prefab, we can't go on
        if (itemContainer == null || rowPrefab == null) return;

        // Clear old rows
        foreach (Transform child in itemContainer) {
            child.SetParent(null); // Move out of container
            Destroy(child.gameObject); // Mark for deletion
        }

        // Built list
        foreach (ItemDefinition item in shopping_list)
        {
            int have = playerInventory != null ? playerInventory.GetTotalItems(item) : 0;
            bool got = have > 0;

            GameObject row = Instantiate(rowPrefab, itemContainer);
            
            row.transform.localScale = Vector3.one; 
            
            TextMeshProUGUI label = row.GetComponent<TextMeshProUGUI>();
            if (label != null) {
                label.text = (got ? "<s>" : "") + item.itemName + (got ? "</s>" : "");
                label.color = got ? Color.green : Color.black;
            }
        }

        // Adjust panel
        if (panel != null) {
            RectTransform rt = panel.GetComponent<RectTransform>();
            if (rt != null) {
                Vector2 pos = rt.anchoredPosition;
                pos.y = (shopping_list.Length * rowHeight) / 2f - verticalOffset;
                rt.anchoredPosition = pos;
            }
        }
    }

    void OnDestroy() {
        if (playerInventory != null) playerInventory.Changed -= RefreshUI;
    }
}