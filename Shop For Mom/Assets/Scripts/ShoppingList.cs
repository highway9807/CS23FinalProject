using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class ShoppingList : MonoBehaviour
{
    [Header("Items required")]
    public ItemDefinition[] shopping_list;

    [Header("UI References")]
    public GameObject panel;//loajsbefoabe
    public Transform itemContainer;
    public GameObject rowPrefab;
    public float rowHeight = 30f;
    public float verticalOffset = 20f;

	//position ietm sin shopping list
	//private List<GameObject> theRows = new List<GameObject>();

    public bool IsOnList(ItemDefinition item)
    {
        if (item == null || shopping_list == null) return false;
        for (int i = 0; i < shopping_list.Length; i++)
            if (shopping_list[i] == item) return true;
        return false;
    }

    private bool isSettingUp = false;

    private PlayerInventory playerInventory;

    void Start()
    {
        ShoppingList existing = FindObjectOfType<ShoppingList>();
        if (existing != null && existing != this)
        {
            existing.shopping_list = this.shopping_list; // copy new scene's list to persistent instance
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
			
			//Create shopping list header
			GameObject rowHeader = Instantiate(rowPrefab, itemContainer);
			rowHeader.transform.localScale = Vector3.one; 
			TextMeshProUGUI labelHeader = rowHeader.GetComponent<TextMeshProUGUI>();
			if (labelHeader != null) {
				labelHeader.text = " Mom's List:";
				labelHeader.color = Color.black;
			}

			//Create all shopping list items for this level 
            foreach (ItemDefinition item in shopping_list)
            {
                int have = playerInventory != null ? playerInventory.GetTotalItems(item) : 0;
                bool got = have > 0;

                GameObject row = Instantiate(rowPrefab, itemContainer);
                
                row.transform.localScale = Vector3.one; 
                
                TextMeshProUGUI label = row.GetComponent<TextMeshProUGUI>();
                if (label != null) {
                    label.text = (got ? "<s>" : "") + " " + item.itemName + (got ? "</s>" : "");
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

		/*
		//for improved legibility /layout:
		//1. Get all items with tag "row" in a list
		theRows.AddRange(GameObject.FindGameObjectsWithTag("Row"));
		//2. in a for-loop, position all these items 10 pixels to the right 
		for (int i=0; i < theRows.Count; i++)
		{
			Vector3 itemPos = theRows[i].transform.localPosition;
			theRows[i].transform.localPosition = new Vector3(itemPos.x + 10f, itemPos.y, itemPos.z);
		}
		*/

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
                if (label != null && label.text.Replace("<s>", "").Replace("</s>", "") == " " + item.itemName) {
                    label.text = (got ? "<s>" : "") + " " + item.itemName + (got ? "</s>" : "");
                    label.color = got ? Color.green : Color.black;
                }

            }
        }

    }

    void OnDestroy() {
        if (playerInventory != null) playerInventory.Changed -= RefreshUI;
    }
}