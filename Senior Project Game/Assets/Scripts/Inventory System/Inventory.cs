using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Inventory 
{
    public event EventHandler onInventoryChanged;
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();

        AddItem(new Item{ itemType = Item.item_type.Meat, amount = 1});
        AddItem(new Item{ itemType = Item.item_type.Weapon, amount = 1});
        AddItem(new Item{ itemType = Item.item_type.Meat, amount = 1});
        AddItem(new Item{ itemType = Item.item_type.Weapon, amount = 1});
        AddItem(new Item{ itemType = Item.item_type.Meat, amount = 1});

        Debug.Log("Inventory Test");
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
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
