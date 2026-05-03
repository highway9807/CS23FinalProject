using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShoppingList : MonoBehaviour
{
    [Header("Items required")]
    public ItemDefinition[] shopping_list;

    [Header("UI References")]
    public GameObject panel;
    public Transform itemContainer;
    public GameObject rowPrefab;
    public float rowHeight = 30f;
    public float verticalOffset = 20f;

    private bool isSettingUp = false;

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
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        itemContainer = null;
        panel = null;
        if(scene.name == "Level0" || scene.name == "Level1" || scene.name == "Level2" || scene.name == "Level3" ||
            scene.name == "Level4" || scene.name == "Level5" || scene.name == "Level6"){
            GameObject [] rows = GameObject.FindGameObjectsWithTag("Row");
            foreach (GameObject row in rows) {
                 // Move out of container
                Destroy(row); // Mark for deletion
            }
            StartCoroutine(SetupAfterLoad(scene));
        }
    }
    IEnumerator SetupAfterLoad(Scene scene)
    {
        isSettingUp = true;
        yield return null;
        FindUIReferences();
        if (itemContainer == null) { isSettingUp = false; yield break; }

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
        

        yield return new WaitForEndOfFrame();

        ContentSizeFitter containerFitter = itemContainer.GetComponent<ContentSizeFitter>();
        if (containerFitter != null) { containerFitter.enabled = false; containerFitter.enabled = true; }

        ContentSizeFitter panelFitter = panel != null ? panel.GetComponent<ContentSizeFitter>() : null;
        if (panelFitter != null) { panelFitter.enabled = false; panelFitter.enabled = true; }

        Canvas.ForceUpdateCanvases();

        if (panel != null) {
            RectTransform rt = panel.GetComponent<RectTransform>();
            if (rt != null) {
                Vector2 pos = rt.anchoredPosition;
                pos.y = (shopping_list.Length * rowHeight) / 2f - verticalOffset;
                rt.anchoredPosition = pos;
            }
        }
        isSettingUp = false;
    }

    void Update()
    {
        // Find UI references if the container was destroyed
        if (!isSettingUp &&(itemContainer == null || !itemContainer.gameObject.scene.IsValid())) {
            FindUIReferences();
        }
    }

    void FindUIReferences()
    {
        panel = GameObject.FindWithTag("Panel");
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

        // Built list
        GameObject [] rows = GameObject.FindGameObjectsWithTag("Row");

        foreach (ItemDefinition item in shopping_list)
        {
            int have = playerInventory != null ? playerInventory.GetTotalItems(item) : 0;
            bool got = have > 0;
            
            foreach(GameObject row in rows){
                TextMeshProUGUI label = row.GetComponent<TextMeshProUGUI>();
                if (label != null && label.text.Replace("<s>", "").Replace("</s>", "") == item.itemName) {
                    label.text = (got ? "<s>" : "") + item.itemName + (got ? "</s>" : "");
                    label.color = got ? Color.green : Color.black;
                }

            }
        }

    }

    void OnDestroy() {
        if (playerInventory != null) playerInventory.Changed -= RefreshUI;
    }
}