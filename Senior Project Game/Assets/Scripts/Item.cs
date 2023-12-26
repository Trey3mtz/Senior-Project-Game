using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    // These are items in the game, mostly relevant to the player and their inventory system.

    // Items are usable objects that the player can hold. This is different from Resources.
    public enum item_type{
        Medicine,
        Weapon,
        CookedFood

    }

    public item_type itemType;
    public int amount;

}
