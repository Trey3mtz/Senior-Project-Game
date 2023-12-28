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
        Meat

    }

    public item_type itemType;
    public int amount;

    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:

            case item_type.Meat:    return Item_Assets.Instance.meatSprite;
            case item_type.Weapon:  return Item_Assets.Instance.weaponSprite;
        }
    }
}