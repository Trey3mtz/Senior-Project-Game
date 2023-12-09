using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    // These are just example item types to have
    public enum item_type{
        Food,
        Medicine,
        Rock,
        Wood

    }

    public item_type itemType;
    public int amount;

}
