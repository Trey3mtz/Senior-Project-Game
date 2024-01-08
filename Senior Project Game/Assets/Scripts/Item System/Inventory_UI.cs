using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.ReorderableList;
using UnityEngine.AI;

namespace Cyrcadian.PlayerSystems.InventorySystem
{

public class Inventory_UI : MonoBehaviour
{ 
    private Inventory inventory;
    [SerializeField] public Transform itemSlotGrid;
    [SerializeField] public Transform hotbarSlotGrid;

    // This is a prefab to be instantiated
    [SerializeField] public Transform inventoryItem;
    [SerializeField] private AudioSource returnSFX;

        private void Awake()
    {
            if (GameManager.Instance == null)
                Debug.Log("[Inv_UI(Awake)] Instance of gamemanager was null ");          
    }

    // onInventoryChanged from the "Inventory" script subscribes to the event
    public void SetInventory(Inventory passedInventory)
    {          
        this.inventory = passedInventory;

        inventory.onInventoryChanged += Inventory_onInventoryChanged;
        RefreshInventoryItems();
        SetInventorySlots();
    }
    
    // Sets the slot inventories index, 0-11 is in the hidden inventory, and 12-16 are in the hotbar
    private void SetInventorySlots()
    {   
        int i = 0;
        foreach(RectTransform Slot in itemSlotGrid)
        {
            Slot.GetComponent<InventorySlot>().slotIndex = i;
            i++;
        }

        foreach(RectTransform Slot in hotbarSlotGrid)
        {
            Slot.GetComponent<QuickSlot>().slotIndex = i;
            i++;
        }
    }

    // An event to be called each time an inventory update happens. For example when something gets added or removed from inventory
    private void Inventory_onInventoryChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryItems();
    }

    // Cleans out all inventory slots, and reinstantiates them based on the item in the saved inventory
    private void RefreshInventoryItems()
    {       // Clean out the slots in inventory UI
            foreach(RectTransform Slot in itemSlotGrid)
            {
                foreach(Transform item in Slot)
                {
                    Destroy(item.gameObject);
                }
            }
            
            // Clean out the slots in hotbar
            foreach(RectTransform Slot in hotbarSlotGrid)
            {
                foreach(Transform item in Slot)
                {
                    Destroy(item.gameObject);
                }
            }            

            // index starts at 0
            // Cache the list of entries from your inventory as we will use its data several times
            int i = 0;
            List<Inventory.InventoryEntry> freshInventory = inventory.GetInventory();

            // This refreshes the hidden inventory, inventory entries 0-11
            foreach(RectTransform Slot in itemSlotGrid)
            {  
                if(freshInventory[i].item != null)
                {
                    RectTransform inventoryItemRectTransform = Instantiate(inventoryItem, Slot).GetComponent<RectTransform>();
                    inventoryItemRectTransform.gameObject.SetActive(true);

                    // This initializes the item in your inventory as a UI Drag and Droppable item element.
                    inventoryItemRectTransform.GetComponent<DragDropItem>().InitializeItem(freshInventory[i].item, i, freshInventory[i].stackSize);

                    TextMeshProUGUI uiText = inventoryItemRectTransform.Find("Item Amount").GetComponent<TextMeshProUGUI>();
                    if(freshInventory[i].stackSize > 1)
                        uiText.SetText("x" + freshInventory[i].stackSize.ToString());
                    else
                        uiText.SetText("");
                }
                i++;
            }
    

            // This refreshes the hotbar, inventory entries 12-16
            foreach(RectTransform Slot in hotbarSlotGrid)
            {  
                if(freshInventory[i].item != null)
                {
                    RectTransform inventoryItemRectTransform = Instantiate(inventoryItem, Slot).GetComponent<RectTransform>();
                    inventoryItemRectTransform.gameObject.SetActive(true);

                    // This initializes the item in your inventory as a UI Drag and Droppable item element.
                    inventoryItemRectTransform.GetComponent<DragDropItem>().InitializeItem(freshInventory[i].item, i, freshInventory[i].stackSize);

                    TextMeshProUGUI uiText = inventoryItemRectTransform.Find("Item Amount").GetComponent<TextMeshProUGUI>();
                    if(freshInventory[i].stackSize > 1)
                        uiText.SetText("x" + freshInventory[i].stackSize.ToString());
                    else
                        uiText.SetText("");
                }
                i++;
            }
    }

    public void DropItemIntoSlot(DragDropItem droppedItem, int slotIndex)
    {   
        List<Inventory.InventoryEntry> currentInventory = inventory.GetInventory();

        // If the slot is preoccupied check if they are the same stackable item, else just replace that index spot
        if(currentInventory[slotIndex].item != null)
        {
            if(currentInventory[slotIndex].item.UniqueID == droppedItem.item.UniqueID && droppedItem.item.IsStackable())
            {// stack them, any left over amount is logically handled by this call already
                inventory.AddItemAt(slotIndex, droppedItem.item, droppedItem.amountStacked);     
                Destroy(droppedItem.gameObject);
            }    
            else
                SwapSlottedItem(droppedItem, slotIndex);
        }
        else
        {
            AddItemIndex(slotIndex, droppedItem.item, droppedItem.amountStacked);
        }
    }

    public void SwapSlottedItem(DragDropItem itemswapped, int slotIndex)
    {
        inventory.SwapIndex(itemswapped.thisIndex, slotIndex);
    }

    public void RemovedItemIndex(int i)
    {
        inventory.RemoveItemAt(i);
    }

    public void AddItemIndex(int i, Item item, int amount)
    {
        inventory.AddItemAt(i, item, amount);
    }

    public void ItemReturnedToSlot(DragDropItem droppedItem, int slotIndex)
    {
        returnSFX.Play();
        DropItemIntoSlot(droppedItem, slotIndex);
    }
}
}