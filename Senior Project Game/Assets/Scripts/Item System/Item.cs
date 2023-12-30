using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian
{

public abstract class Item : ScriptableObject, IDatabaseEntry
{
    // These are items in the game, mostly relevant to the player and their inventory system.

    // Items are usable objects that the player can hold. This is different from Resources.
    public enum item_type{
        Medicine,
        Weapon,
        Meat
    }

    public string Key => UniqueID;

    [Tooltip("Name used in the database for that Item, used by save system so no [spaces]")]
    public string UniqueID = "DefaultID";

    [Tooltip("Display name that will be visible in-game, allowed to have [spaces] in the name")]        
    public string DisplayName;
    public Sprite ItemSprite;
    public int MaxStackSize = 32;
    public bool Consumable = true;


    [Tooltip("Prefab that will be instantiated in the player hand when this is equipped")]
    public GameObject VisualPrefab;
    public string PlayerAnimatorTriggerUse = "GenericToolSwing";

    [Tooltip("Sound triggered when using the item")]
    public AudioClip[] UseSound;

    public item_type itemType;
    public int amount;

    // CanUse needs to see if it can be used on this tile, Use is the abstract logic of using that item
    public abstract bool CanUse(Vector3Int target);
    public abstract bool Use(Vector3Int target);

    //override this for item that needs a specific target (Like placing stationary box on ground tiles)
    public virtual bool NeedTarget()
    {
        return false;
    }    


    public bool IsStackable()
    {
        return MaxStackSize > 1;
    }
}

}