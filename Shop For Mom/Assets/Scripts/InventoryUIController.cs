using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private List<Image> slotIcons = new List<Image>();
    [SerializeField] private List<Image> slotFrames = new List<Image>();
    [Header("Feedback")]
    [SerializeField] private Color normalSlotColor = Color.white;
    [SerializeField] private Color fullFlashColor = Color.red;
    [SerializeField] private float flashSecs = 0.2f;
    private PlayerInventory playerInventory;
    private Coroutine flashing;

    // Name: Start
    // Purpose: Initialize inventory source and render current slot icons.
    // Inputs: None.
    // Outputs: None.
    private void Start()
    {
        if (inventoryPanel == null)
            inventoryPanel = gameObject;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        playerInventory = FindPlayerInventory();
        if (playerInventory != null)
            playerInventory.Changed += RefreshInventory;
        RefreshInventory();
    }
    
    // Name: FindPlayerInventory
    // Purpose: Locate the PlayerInventory backend used by this UI.
    // Inputs: None.
    // Outputs: PlayerInventory reference, or null if none exists.
    private PlayerInventory FindPlayerInventory()
    {
        if (GameHandler.gh != null && GameHandler.gh.PlayerInventory != null)
            return GameHandler.gh.PlayerInventory;
        return FindFirstObjectByType<PlayerInventory>();
    }
    
    // Name: OnDestroy
    // Purpose: Unsubscribe from inventory updates before object is destroyed.
    // Inputs: None.
    // Outputs: None.
    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.Changed -= RefreshInventory;
    }
    
    // Name: RefreshInventory
    // Purpose: Copy backend inventory items into the 10 HUD icon slots.
    // Inputs: None.
    // Outputs: None.
    private void RefreshInventory()
    {
        List<ItemDefinition> items = null;
        if (playerInventory != null)
            items = playerInventory.GetItemSlots();
    
        for (int i = 0; i < slotIcons.Count; i++)
        {
            Image icon = slotIcons[i];
            if (icon == null)
                continue;
    
            if (items != null && i < items.Count && items[i] != null && items[i].sprite != null)
            {
                icon.enabled = true;
                icon.sprite = items[i].sprite;
                icon.preserveAspect = true;
            }
            else
            {
                icon.sprite = null;
                icon.enabled = false;
            }
        }
    }
    
    // Name: PlayFullInventoryFeedback
    // Purpose: Trigger the brief red flash when pickup is blocked.
    // Inputs: None.
    // Outputs: None.
    public void PlayFullInventoryFeedback()
    {
        if (flashing != null)
            StopCoroutine(flashing);
        flashing = StartCoroutine(FlashSlotsRed());
    }
    
    // Name: FlashSlotsRed
    // Purpose: Temporarily tint slot frames red, then restore old colors.
    // Inputs: None.
    // Outputs: IEnumerator used by coroutine scheduler.
    private IEnumerator FlashSlotsRed()
    {
        SetAllSlotFrameColors(fullFlashColor);
        yield return new WaitForSeconds(flashSecs);
        SetAllSlotFrameColors(normalSlotColor);
        flashing = null;
    }

    // Name: SetAllSlotFrameColors
    // Purpose: Apply one color to every slot frame in the HUD.
    // Inputs: color - the tint to apply to each slot frame image.
    // Outputs: None.
    private void SetAllSlotFrameColors(Color color)
    {
        for (int i = 0; i < slotFrames.Count; i++)
        {
            if (slotFrames[i] != null)
                slotFrames[i].color = color;
        }
    }
}
