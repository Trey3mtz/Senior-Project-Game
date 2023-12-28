using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class Collect_World_Item : MonoBehaviour
{
    
    private Inventory inventory;

    // Player Controller sets the inventory and calls this to set it,
    //      so that the items collected go to the player's inventory
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        World_Item item = collider.GetComponent<World_Item>();
        if(item != null)
        {
            inventory.AddItem(item.GetItem());
            item.DestroySelf();
        }
    }
}
