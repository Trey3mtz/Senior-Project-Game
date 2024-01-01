using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian
{

// This class is really for testing purposes. It will spawn the item specified in the inspector.
public class World_Item_SPAWNER : MonoBehaviour
{
    public Item item;

    private void Start()
    {
        World_Item.SpawnWorldItem(transform.position, item);
        //Destroy(gameObject);
    }
    
}
}