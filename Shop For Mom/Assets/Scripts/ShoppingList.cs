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
    private bool visible = false;

    void Start()
    {
        if (GameHandler.gh != null)
        {
            playerInventory = GameHandler.gh.PlayerInventory;
            playerInventory.Changed += RefreshUI;
        }

        panel.SetActive(false);
        RefreshUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            visible = !visible;
            panel.SetActive(visible);
            if (visible) RefreshUI();
        }
    }

    void RefreshUI()
    {
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
            label.color = got ? Color.green : Color.white;
        }
    }

    void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.Changed -= RefreshUI;
    }
}
