using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    // Scripts call this event whenever the inventory changes so UI can update.
    // note: Action denotes a void return with no parameter input
    // in C/C++ terms, this behaves like a list of function pointers called in
    // order when this object/file calls Changed().
    // THIS class controls when the event is called, but other scripts decide
    // what functions should run when it happens by "subscribing". Assuming we
    // declared:
    //     PlayerInventory playerInv;
    // To Subscribe:
    //     playerInv.Changed += SomeFunction;
    // To Unsubscribe:
    //     playerInv.Changed -= SomeFunction;
    // when we invoke Changed(), every subscribed function gets called 
    // immediately.
    public event Action Changed;

    // create a list of slots (dynamic). serialize the field so we can change it in the editor.
    [SerializeField]
    private List<ItemDefinition> slots = new List<ItemDefinition>();

    // Name: GetItemSlots
    // Purpose: Expose the current inventory slot list for read-only inspection
    //          (UI, scoring, etc.).
    // Inputs:  None.
    // Outputs: The internal list of ItemDefinition references in slot order 
    public List<ItemDefinition> GetItemSlots()
    {
        return slots;
    }

    // Name: GetTotalItems
    // Purpose: Count how many slots reference the given item (total copies of 
    //          that item).
    // Inputs:  item — the ItemDefinition to count; null is treated as zero 
    //          matches.
    // Outputs: Non-negative count of slots where slots[i] == item.
    public int GetTotalItems(ItemDefinition item)
    {

        if (item == null) return 0;

        int itemTotal = 0;
        int slotCount = slots.Count;
        for (int i = 0; i < slotCount; i++)
        {   

            ItemDefinition currItem = slots[i];
            if (currItem == item)
                itemTotal++;
            
        }

        return itemTotal;
    }

    // Name: ClearAll
    // Purpose: Remove every entry from the inventory and notify listeners.
    // Inputs:  None.
    // Outputs: None (side effect: slots empty, Changed invoked if subscribed).
    public void ClearAll()
    {
        slots.Clear();
        NotifyChanged();
    }

    // Name: TryAdd
    // Purpose: Append one instance of an item to the inventory and notify 
    //          listeners on success.
    // Inputs: item — the ItemDefinition to add; null is rejected.
    // Outputs: True if the item was added; false if item was null.
    public bool TryAdd(ItemDefinition item)
    {
        if (item == null)
            return false;

        slots.Add(item);
        NotifyChanged();
        return true;
    }

    // Name: TryRemove
    // Purpose: Remove one slot that matches the given item (searches from the
    //          end of the list) and notify listeners if a slot was removed.
    // Inputs:  item — the ItemDefinition to remove one copy of; null or absent
    //          item yields false. 
    // Outputs: True if one matching slot was removed; false if nothing was 
    //          removed.
    public bool TryRemove(ItemDefinition item)
    {

        if ((item == null) || (GetTotalItems(item) <= 0))
            return false;

        int slotsMaxIndex = slots.Count - 1;
        for(int i = slotsMaxIndex; i >= 0; i--)
        {
            if (slots[i] == item)
            {
                slots.RemoveAt(i);
                NotifyChanged();
                return true;
            }
        }

        Debug.Log("Item Removal Error");
        return false;
    }

    // Name: NotifyChanged
    // Purpose: Raise the Changed event so subscribers (e.g. UI) can refresh.
    // Inputs:  None.
    // Outputs: None.
    private void NotifyChanged()
    {
        Changed?.Invoke();
    }

    // Name: PrintInventory
    // Purpose: Print the inventory to console while we work on UI
    // Inputs:  None.
    // Outputs: Prints the contents of the current playerinventory to console.
    public void printInventory()
    {
        string invString = "---------------------------------------------" +
                           "Inventory" +
                           "---------------------------------------------\n";
        for(int i = 0; i < slots.Count; i++)
        {
            invString += "Slot " + i + ": " + slots[i].itemName;
            if (((i + 1) % 5) == 0)
            {
                invString += "\n";
            } else
            {
                invString += ", ";
            }
        }
        Debug.Log(invString);
    }
}
