using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing_Inventory_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        World_Item.SpawnWorldItem(new Vector3(-10, 5), new Item{ itemType = Item.item_type.Meat, amount = 1});
        World_Item.SpawnWorldItem(new Vector3(-5, 15), new Item{ itemType = Item.item_type.Meat, amount = 1});
        World_Item.SpawnWorldItem(new Vector3(-10, 15), new Item{ itemType = Item.item_type.Meat, amount = 1});
    }

}
