using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private List<Image> slotIcons = new List<Image>();
    [SerializeField] private List<Button> slotButtons = new List<Button>();
    [Header("Feedback")]
    [SerializeField] private Color normalSlotColor = Color.white;
    [SerializeField] private Color hoverSlotColor = Color.yellow;
    [SerializeField] private Color pressedSlotColor = Color.white;
    [SerializeField] private Color selectedSlotColor = Color.yellow;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashSecs = 0.2f;
    private PlayerInventory playerInventory;
    private Coroutine flashing;

    private bool debugFlag = true;

    // Name: OnEnable
    // Purpose: Bind to the active PlayerInventory whenever this UI is enabled.
    //          Using OnEnable (not only Start) avoids missing GameHandler.gh or
    //          sceneLoaded timing where Changed fired before we subscribed.
    // Inputs: None.
    // Outputs: None.
    private void OnEnable()
    {
        if (inventoryPanel == null)
            inventoryPanel = gameObject;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        if (playerInventory != null)
            playerInventory.Changed -= RefreshInventory;
        playerInventory = FindPlayerInventory();
        if (playerInventory != null)
            playerInventory.Changed += RefreshInventory;
        SetupButtonColors();
        RefreshInventory();
    }

    private void OnDisable()
    {
        if (playerInventory != null)
            playerInventory.Changed -= RefreshInventory;
    }
    
    // Name: FindPlayerInventory
    // Purpose: Locate the PlayerInventory backend used by this UI.
    // Inputs: None.
    // Outputs: PlayerInventory reference, or null if none exists.
    private PlayerInventory FindPlayerInventory()
    {
        if (GameHandler.gh != null)
        {
            PlayerInventory ghInv = GameHandler.gh.PlayerInventory;
            if (ghInv != null)
                return ghInv;
        }
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
                // Icon lives on a child GameObject that is inactive in the slot prefab until we
                // assign an item; Image.enabled alone does not render if the GameObject is off.
                icon.gameObject.SetActive(true);
                icon.enabled = true;
                icon.sprite = items[i].sprite;
                icon.preserveAspect = true;
            }
            else
            {
                icon.sprite = null;
                icon.enabled = false;
                icon.gameObject.SetActive(false);
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
        SetAllSlotFrameColors(flashColor);
        yield return new WaitForSeconds(flashSecs);
        SetAllSlotFrameColors(normalSlotColor);
        flashing = null;
    }

    // Name: SetAllSlotFrameColors
    // Purpose: Apply one normal-state color to every inventory slot button.
    // Inputs: color - the normal tint to apply to each slot button.
    // Outputs: None.
    private void SetAllSlotFrameColors(Color color)
    {
        for (int i = 0; i < slotButtons.Count; i++)
        {
            if (slotButtons[i] == null)
                continue;

            ColorBlock cb = slotButtons[i].colors;
            cb.normalColor = color;
            slotButtons[i].colors = cb;
        }
    }

    // Name: SetupButtonColors
    // Purpose: Configure button hover/pressed/selected colors once on startup.
    // Inputs: None.
    // Outputs: None.
    private void SetupButtonColors()
    {
        for (int i = 0; i < slotButtons.Count; i++)
        {
            if (slotButtons[i] == null)
                continue;

            ColorBlock cb = slotButtons[i].colors;
            cb.normalColor = normalSlotColor;
            cb.highlightedColor = hoverSlotColor;
            cb.pressedColor = pressedSlotColor;
            cb.selectedColor = normalSlotColor;
            slotButtons[i].colors = cb;
        }
    }
}
