using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Entities;
using UnityEngine;

namespace Cyrcadian
{
    [Serializable]
    public class Inventory 
    {
        public event EventHandler onInventoryChanged;
        public const int initialInventorySize = 12;


        [Serializable]
        public class InventoryEntry
        {
            public Item item;
            public int stackSize;
        }

        public List<InventoryEntry> _Inventory = new List<InventoryEntry>(12);

        public void InitializeInventory()
        {
            for(int i = 0; i < initialInventorySize; i++)
                _Inventory.Add(new InventoryEntry());
        }

        public List<InventoryEntry> GetInventory()
        {   
            return _Inventory;
        }

        // Current limitation: it will add more than the item's limit in some cases, add a case for "stacksize+amountAdd > MaxStackSize" later
        public bool AddItem(Item newItem,  int amountAdd)
        {   
            bool foundSpace = false;
            int openSlot = _Inventory.FindIndex(e => e.item == null);

            if(newItem.IsStackable())
            {
                bool isAlreadyInInventory = false;

                foreach(InventoryEntry entry in _Inventory)
                {
                    if(entry.item != null)
                        if(entry.item.UniqueID == newItem.UniqueID && entry.stackSize < entry.item.MaxStackSize)
                        {   
                            entry.item = newItem;
                            entry.stackSize += amountAdd;
                           
                            isAlreadyInInventory = true;
                            foundSpace = true;
                            // There existed a stack of this item, and there was space.
                        }
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

        public void SwapIndex(int oldIndex, int newIndex)
        {
            InventoryEntry tempEntry = _Inventory[newIndex];

            _Inventory[newIndex] = _Inventory[oldIndex];
            _Inventory[oldIndex] = tempEntry;
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

        public void ConsumeItem(Item item)
        {
        
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