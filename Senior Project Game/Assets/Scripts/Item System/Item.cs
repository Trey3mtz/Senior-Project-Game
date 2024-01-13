using Unity.Entities;
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

    // NOTE: This is just string for a tooltip. You must place Tooltip_Trigger on
    //       whatever you want to have a tooltip, and grab the item's Tooltip strings
    //       and set that Tooltip_Tigger's strings to these string.
    [Tooltip("This is a tooltip to display item's name, and description if needed")]  
    public string Tooltip_header;
    [Multiline()]
    public string Tooltip_content;

    [Tooltip("Prefab that will be instantiated in the player hand when this is equipped")]
    public GameObject VisualPrefab;
    public string PlayerAnimatorTriggerUse = "GenericToolSwing";

    [Tooltip("This is the prefab that you will see when it is dropped into the world, out of inventory")]
    public GameObject WorldItemPrefab;

    [Tooltip("Sound triggered when using the item")]
    public AudioClip[] UseSound;
    public Vector2 volume = new Vector2(0.5f, 0.5f);
    public Vector2 pitch = new Vector2(1,1);

    public item_type itemType;

    // CanUse needs to see if it can be used on this tile, Use is the abstract logic of using that item
    public abstract bool CanUse(Vector3Int target);
    public abstract bool Use(Vector3Int target, GameObject gameObject);

    //override this for item that needs a specific target (Like placing stationary box on ground tiles)
    public virtual bool NeedTarget()
    {
        return false;
    }    


    public bool IsStackable()
    {
        return MaxStackSize > 1;
    }

    public AudioSource PlaySound(AudioSource audioSourceParam = null)
    {
        if(UseSound.Length == 0)
        {
            Debug.LogWarning($"Missing sound cliops for item {DisplayName}");
            return null;
        }

        var source = audioSourceParam;
        if(source == null)
        {
            var obj = new GameObject("Sound", typeof(AudioSource));
            source = obj.GetComponent<AudioSource>();
        }

        // Set source config
        source.clip = UseSound[0];
        source.Play();
        Destroy(source.gameObject, source.clip.length/source.pitch);

        return source;
    }
}

}