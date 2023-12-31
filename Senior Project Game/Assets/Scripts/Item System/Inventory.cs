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
        public void Awake()
        {
            Debug.Log("Inventory is awake");
        }

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
            for (int i = 0; i < data.Count; ++i)
            {
                if (data[i] != null)
                {
                    _Inventory[i].item = GameManager.Instance.ItemDatabase.GetFromID(data[i].ItemID);
                    _Inventory[i].stackSize = data[i].AmountHeld;
                }
                else
                {
                    _Inventory[i].item = null;
                    _Inventory[i].stackSize = 0;
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