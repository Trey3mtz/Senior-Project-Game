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
    [SerializeField] public Transform inventoryItem; 

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

    private void SetInventorySlots()
    {   
        int i = 0;
        foreach(RectTransform Slot in itemSlotGrid)
        {
            Slot.GetComponent<InventorySlot>().slotIndex = i;
            i++;
        }
    }

    // An event to be called each time an inventory update happens. For example when something gets added or removed from inventory.
    private void Inventory_onInventoryChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {       // Clean out the slots in inventory UI
            foreach(RectTransform Slot in itemSlotGrid)
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
    }

    public void ChangedSlottedItem(int a, int b)
    {
        inventory.SwapIndex(a, b);
    }

    public void RemovedItemIndex(int i)
    {
        inventory.RemoveItemAt(i);
    }
}
}