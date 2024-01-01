using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Cyrcadian
{
    [Serializable]
    public class Inventory 
    {
        public const int InventorySize = 12;
        public event EventHandler onInventoryChanged;

        [Serializable]
        public class InventoryEntry
        {
            public Item item;
            public int stackSize;
        }

        public List<InventoryEntry> _Inventory = new List<InventoryEntry>();

        public List<InventoryEntry> GetInventory()
        {   
            return _Inventory;
        }

        public void AddItem(Item newItem,  int amountAdd)
        {
            if(newItem.IsStackable())
            {
                bool isAlreadyInInventory = false;
                foreach(InventoryEntry inventoryEntry in _Inventory)
                {
                    if(inventoryEntry.item.UniqueID == newItem.UniqueID && inventoryEntry.stackSize <= inventoryEntry.item.MaxStackSize)
                    {     
                        inventoryEntry.stackSize += amountAdd;
                        isAlreadyInInventory = true;
                    }
                }
                if(!isAlreadyInInventory)
                {
                    _Inventory.Add(new InventoryEntry(){ item = newItem, stackSize = amountAdd });
                }   
            }
            else
            {
                _Inventory.Add(new InventoryEntry(){ item = newItem, stackSize = amountAdd });
            }
                
            onInventoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItem(InventoryEntry entry)
        {
            _Inventory.Remove(entry);
            //_Inventory.TrimExcess();
            onInventoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItemAt(int entryIndex)
        {
            _Inventory.RemoveAt(entryIndex);
            //_Inventory.TrimExcess();
            onInventoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ConsumeItem(Item item)
        {
        
        }

        
        // Save the content of the inventory in the given list.
        public void Save(ref List<InventorySaveData> data)
        {
            foreach (InventoryEntry entry in _Inventory)
            {
                if (entry.item != null)
                {
                    data.Add(new InventorySaveData()
                    {
                        ItemID = entry.item.UniqueID,
                        AmountHeld = entry.stackSize                        
                    });
                }
                else
                {
                    data.Add(null);
                }
            }
        }

        // Load the content in the given list inside that inventory.
        public void Load(List<InventorySaveData> data)
        { 
            _Inventory.Clear();
            foreach (InventorySaveData entry in data)
            {
                if (entry.ItemID != null)
                {
                    _Inventory.Add(new InventoryEntry()
                     {
                        item = GameManager.Instance.ItemDatabase.GetFromID(entry.ItemID),
                        stackSize = entry.AmountHeld
                     });
                }
                else
                {
                   _Inventory.Add(null);
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