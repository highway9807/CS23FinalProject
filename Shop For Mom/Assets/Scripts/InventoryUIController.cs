using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{

    /* TODO: replace this scene name with a list of viable scenes once we have
     * them */
    [Header("Scene Restriction")]
    [SerializeField] private string targetSceneName = "Level1";

    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private ScrollRect inventoryScrollRect;
    [SerializeField] private RectTransform viewportRect;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private GameObject slotPrefab;

    [Header("Layout")]
    [SerializeField] private int columns = 5;
    [SerializeField] private int visibleRows = 5;
    [SerializeField] private Vector2 spacing = new Vector2(8f, 8f);
    [SerializeField] private RectOffset padding;
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;
    

    private PlayerInventory playerInventory;
    private GridLayoutGroup gridLayout;
    private CanvasGroup panelCanvasGroup;
    private readonly List<GameObject> spawnedSlots = new List<GameObject>();

    // Name: Awake
    // Purpose: Initialize safe defaults that cannot be created in field initializers.
    // Inputs: None.
    // Outputs: None.
    private void Awake()
    {
        if (padding == null)
            padding = new RectOffset(8, 8, 8, 8);
    }

    // Name: Start
    // Purpose: Initialize references, lock this feature to Level1, and hide 
    //          inventory on start.
    // Inputs: None.
    // Outputs: None.
    private void Start()
    {
        // Keep this UI disabled in non-Level1 scenes.
        if (!IsTargetScene())
        {
            LogDebug("Disabled: active scene does not match target scene.");
            if (inventoryPanel != null) inventoryPanel.SetActive(false);
            enabled = false;
            return;
        }

        playerInventory = ResolvePlayerInventory();
        SetupLayoutComponents();

        if (inventoryPanel != null)
            SetPanelVisible(false);
        else
            LogDebug("InventoryPanel is NULL.");

        if (playerInventory != null)
            // Rebuild UI whenever backend inventory changes.
            playerInventory.Changed += RefreshInventory;
        else
            LogDebug("PlayerInventory is NULL.");

        LogDebug("Controller initialized.");
    }

    // Name: OnDestroy
    // Purpose: Unsubscribe from inventory changed event.
    // Inputs: None.
    // Outputs: None.
    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.Changed -= RefreshInventory;
    }

    // Name: Update
    // Purpose: Toggle inventory UI when player presses I in Level1.
    // Inputs: Keyboard input (KeyCode.I).
    // Outputs: None.
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.I))
            return;

        LogDebug("Detected I key press.");

        if (inventoryPanel == null)
        {
            LogDebug("Toggle skipped: inventoryPanel is NULL.");
            return;
        }

        bool nextState = panelCanvasGroup == null || panelCanvasGroup.alpha <= 0.01f;
        SetPanelVisible(nextState);
        LogDebug("Inventory panel active state is now: " + nextState);

        // Refresh on open so shown data is always current.
        if (nextState)
            RefreshInventory();
    }

    // Name: IsTargetScene
    // Purpose: Check if the current scene is the intended scene for this 
    //          inventory UI.
    // Inputs: None.
    // Outputs: True only when active scene name matches targetSceneName.
    private bool IsTargetScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        return string.Equals(activeSceneName, targetSceneName, StringComparison.OrdinalIgnoreCase);
    }

    // Name: ResolvePlayerInventory
    // Purpose: Find the backend PlayerInventory to read slots from.
    // Inputs: None.
    // Outputs: PlayerInventory reference or null.
    private PlayerInventory ResolvePlayerInventory()
    {
        if (GameHandler.gh != null && GameHandler.gh.PlayerInventory != null)
            return GameHandler.gh.PlayerInventory;

        return FindFirstObjectByType<PlayerInventory>();
    }

    // Name: SetupLayoutComponents
    // Purpose: Configure GridLayout + content sizing so we show 5x5 visible 
    //          cells and scroll overflow.
    // Inputs: None.
    // Outputs: None.
    private void SetupLayoutComponents()
    {
        if (contentRect == null)
            return;

        if (inventoryPanel != null)
        {
            panelCanvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
                panelCanvasGroup = inventoryPanel.AddComponent<CanvasGroup>();
        }

        gridLayout = contentRect.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
            gridLayout = contentRect.gameObject.AddComponent<GridLayoutGroup>();

        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.spacing = spacing;
        gridLayout.padding = padding;
        gridLayout.childAlignment = TextAnchor.UpperLeft;

        ContentSizeFitter fitter = contentRect.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = contentRect.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        if (inventoryScrollRect != null)
        {
            inventoryScrollRect.horizontal = false;
            inventoryScrollRect.vertical = true;
            inventoryScrollRect.content = contentRect;
            if (viewportRect != null)
                inventoryScrollRect.viewport = viewportRect;
        }

        UpdateCellSize();
    }

    // Name: UpdateCellSize
    // Purpose: Calculate a cell size that fits exactly 5 columns and 5 rows in 
    //          the viewport.
    // Inputs: None.
    // Outputs: None.
    private void UpdateCellSize()
    {
        if (gridLayout == null || viewportRect == null)
            return;

        // Remove padding/spacing before dividing into a fixed 5x5 visible area.
        float widthInside = viewportRect.rect.width - padding.left 
                            - padding.right - spacing.x * (columns - 1);
        float heightInside = viewportRect.rect.height - padding.top 
                            - padding.bottom - spacing.y * (visibleRows - 1);

        float cellWidth = Mathf.Max(1f, widthInside / columns);
        float cellHeight = Mathf.Max(1f, heightInside / visibleRows);
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
    }

    // Name: RefreshInventory
    // Purpose: Rebuild slot UI to match current PlayerInventory content.
    // Inputs: None.
    // Outputs: None.
    private void RefreshInventory()
    {
        if (playerInventory == null || contentRect == null || slotPrefab == null)
        {
            LogDebug("Refresh skipped due to missing refs (inventory/content/slotPrefab).");
            return;
        }

        UpdateCellSize();

        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            if (spawnedSlots[i] != null)
                Destroy(spawnedSlots[i]);
        }
        spawnedSlots.Clear();

        List<ItemDefinition> items = playerInventory.GetItemSlots();
        LogDebug("Refreshing inventory. Items in backend list: " + items.Count);
        for (int i = 0; i < items.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, contentRect);
            spawnedSlots.Add(slot);

            // ItemDefinition owns the icon sprite used in the inventory.
            ItemDefinition item = items[i];
            Image icon = FindSlotIcon(slot);
            if (icon != null && item != null && item.sprite != null)
            {
                icon.enabled = true;
                icon.sprite = item.sprite;
                icon.preserveAspect = true;
            }
            else if (icon != null)
            {
                icon.enabled = false;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        Canvas.ForceUpdateCanvases();
        if (inventoryScrollRect != null)
            inventoryScrollRect.verticalNormalizedPosition = 1f;
    }

    // Name: FindSlotIcon
    // Purpose: Locate an Image component used to display item art in a slot 
    //          prefab.
    // Inputs: Slot game object instance.
    // Outputs: Image for icon display (or null if not found).
    private Image FindSlotIcon(GameObject slot)
    {
        Transform iconChild = slot.transform.Find("Icon");
        if (iconChild != null)
            return iconChild.GetComponent<Image>();
        
        Debug.LogWarning("Inventory slot prefab is missing a child named 'Icon'.");
        return null;
    }

    // Name: SetPanelVisible
    // Purpose: Show/hide panel without disabling the object that may hold this script.
    // Inputs: visible - true to show panel, false to hide it.
    // Outputs: None.
    private void SetPanelVisible(bool visible)
    {
        if (inventoryPanel == null || panelCanvasGroup == null)
            return;

        panelCanvasGroup.alpha = visible ? 1f : 0f;
        panelCanvasGroup.interactable = visible;
        panelCanvasGroup.blocksRaycasts = visible;
    }

    // Name: LogDebug
    // Purpose: Emit optional debug logs for inventory UI setup and toggling.
    // Inputs: message - text to print.
    // Outputs: None.
    private void LogDebug(string message)
    {
        if (!enableDebugLogs)
            return;

        Debug.Log("[InventoryUIController] " + message);
    }
}
