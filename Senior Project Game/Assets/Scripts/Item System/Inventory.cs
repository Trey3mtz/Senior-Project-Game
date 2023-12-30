using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Cyrcadian
{

public class Inventory 
{
    public event EventHandler onInventoryChanged;
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();
        Debug.Log("Inventory Test");
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public void AddItem(Item item)
    {
        if(item.IsStackable())
        {
            bool isAlreadyInInventory = false;
            foreach(Item inventoryItem in itemList)
            {
                if(inventoryItem.UniqueID == item.UniqueID && inventoryItem.amount <= inventoryItem.MaxStackSize)
                {
                    inventoryItem.amount += item.amount;
                    isAlreadyInInventory = true;
                }
            }
            if(!isAlreadyInInventory)   
                itemList.Add(item); 
        }
        else
        {
            itemList.Add(item);
        }
            
        onInventoryChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItem(Item item)
    {
         itemList.Remove(item);
         onInventoryChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ConsumeItem(Item item)
    {
    
    }
}
}