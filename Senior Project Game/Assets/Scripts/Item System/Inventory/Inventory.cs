using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cyrcadian.PlayerSystems.InventorySystem;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Cyrcadian
{
    [Serializable]
    public class Inventory 
    {
        public event EventHandler onInventoryChanged;
        public const int initialInventorySize = 17;


        [Serializable]
        public class InventoryEntry
        {
            public Item item;
            public int stackSize;
        }

        // Inventory entries 0 - 11 are in regular inventory. Entries 12-16 are in Hotbar Quick access.
        private List<InventoryEntry> _Inventory = new List<InventoryEntry>(initialInventorySize);

        public void InitializeInventory()
        {
            for(int i = 0; i < initialInventorySize; i++)
                _Inventory.Add(new InventoryEntry());
        }

        public List<InventoryEntry> GetInventory()
        {   
            return _Inventory;
        }

        // This is for when you automatically suck up items.
        public bool AddItem(Item newItem,  int amountAdd)
        {   
            bool foundSpace = false;
            int openSlot = _Inventory.FindIndex(e => e.item == null);
            
            if(newItem.IsStackable())
            {
                bool isAlreadyInInventory = false;
                int i=0;
                foreach(InventoryEntry entry in _Inventory)
                {
                    if(entry.item != null && !isAlreadyInInventory)
                        if(entry.item.UniqueID == newItem.UniqueID && entry.stackSize < entry.item.MaxStackSize)
                        { 
                            if(entry.stackSize + amountAdd <= entry.item.MaxStackSize)
                             {
                                entry.stackSize += amountAdd;
                                isAlreadyInInventory = true;
                                foundSpace = true;
                             }   
                            else
                            {   // Find the amount spilled over the max, set the stacksize to the max, and do something with the left over
                                int amountOver = entry.stackSize + amountAdd - entry.item.MaxStackSize; 
                                entry.stackSize = entry.item.MaxStackSize;
                                isAlreadyInInventory = true;

                                if(!AddItem(newItem, amountOver))
                                {
                                    //{ INVENTORY HAS NO MORE SPACE for the left over } 
                                } 
                                else
                                    foundSpace = true;    
                            }
                            // There existed a stack of this item, and there was some space.
                        }i++;
                }

                if(!isAlreadyInInventory && openSlot != -1)
                {
                        _Inventory[openSlot].item = newItem;
                        _Inventory[openSlot].stackSize = amountAdd;

                    foundSpace = true;
                    // This item is stackable, but there was no stack yet, and there was space.
                }   
            }
            else if(openSlot != -1)
            {
                    _Inventory[openSlot].item = newItem;
                    _Inventory[openSlot].stackSize = amountAdd;

                foundSpace = true;
                // The item was not stackable, but there was space
            }
   
            if(foundSpace)
                onInventoryChanged?.Invoke(this, EventArgs.Empty);

            return foundSpace;
        }

        // This is for when you are choosing which slot to add an item into. Creates a new Entry and decides how it should be added.
        public void AddItemAt(int entryIndex, Item newItem,  int amountAdd)
        {
            InventoryEntry newEntry = new InventoryEntry(){ item = newItem, stackSize = amountAdd};

            // If nothing was at index, replace the index with this entry. If there was something, see if they can stack.
            if(_Inventory[entryIndex].item == null)
                _Inventory[entryIndex] = newEntry;
            else
            { Debug.Log("add " +newItem.UniqueID+" to index " + entryIndex);
                if(_Inventory[entryIndex].item.UniqueID == newItem.UniqueID && newItem.IsStackable())
                {  
                    if(_Inventory[entryIndex].stackSize + amountAdd <= newItem.MaxStackSize)
                            _Inventory[entryIndex].stackSize += amountAdd;  
                    else
                    {   // If there is left over stackamounts, fill up the first stack, and find any spot for the leftover
                        int amountOver = _Inventory[entryIndex].stackSize + amountAdd - newItem.MaxStackSize; 
                        _Inventory[entryIndex].stackSize = newItem.MaxStackSize;

                        if(!AddItem(newItem, amountOver))
                        {
                            // Aboslutely full, no room left for the leftover amount

                            // Instantiate a world item and its amount to drop out of inventory 
                            //  not necesary rn as you can't pick up items when no room
                        }
                    }
                }
                //else { Swap items spots logic handled already in UI's logic}       
            }

            onInventoryChanged?.Invoke(this, EventArgs.Empty);
        }

        // oldIndex is empty as it was dragged out of the slot.
        // oldIndex will get filled with the newIndex we want to place an item in
        // We place our DragDropItem into the newIndex
        public void SwapIndex(int oldIndex, int newIndex, DragDropItem droppedItem)
        {       
            _Inventory[oldIndex] = _Inventory[newIndex];
            _Inventory[newIndex] = new InventoryEntry()
            {   item = droppedItem.item, 
                stackSize = droppedItem.amountStacked};

            onInventoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItem(InventoryEntry entry)
        {
            _Inventory.Remove(entry);
            
            onInventoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItemAt(int entryIndex)
        {
            _Inventory[entryIndex] = new InventoryEntry();
            
            onInventoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ConsumeItem(int entryIndex)
        {
            _Inventory[entryIndex].stackSize -= 1;

            if(_Inventory[entryIndex].stackSize <= 0)
                RemoveItemAt(entryIndex);

            onInventoryChanged?.Invoke(this, EventArgs.Empty); 
        }

        
        // Save the content of the inventory in the given list.
        public void Save(ref List<InventorySaveData> data)
        {
            for(int i = 0; i < _Inventory.Capacity; i++)
            {
                if (_Inventory[i].item != null)
                {
                    data.Add(new InventorySaveData()
                    {
                        ItemID = _Inventory[i].item.UniqueID,
                        AmountHeld = _Inventory[i].stackSize                        
                    });
                }
                else
                {
                    data.Add(new InventorySaveData());
                }
            }
        }

        // Load the content in the given list inside that inventory.
        public void Load(List<InventorySaveData> data)
        { 
            _Inventory.Clear();
            for(int i = 0; i < _Inventory.Capacity; i++)
            {
                if (data[i].ItemID != null)
                {
                    _Inventory.Add(new InventoryEntry()
                     {
                        item = GameManager.Instance.ItemDatabase.GetFromID(data[i].ItemID),
                        stackSize = data[i].AmountHeld
                     });
                }
                else
                {
                   _Inventory.Add(new InventoryEntry());
                }
            }
        }
    }

[Serializable]
public class InventorySaveData
{
    public string ItemID;
    public int AmountHeld;
}
}