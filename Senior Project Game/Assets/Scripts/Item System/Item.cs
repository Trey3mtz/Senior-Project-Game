using Unity.Entities;
using UnityEngine;

namespace Cyrcadian
{

    public abstract class Item : ScriptableObject, IDatabaseEntry
    {
        // These enum are for the purpose of sorting items
        // The enums at the Top will be first, in the top left of the inventory.
        // The enums at the Bottom of this list will go to the bottom of the inventory
        public enum ItemType{
            Tool,
            Weapon,
            Medicine,
            Food,
            other
        }


        public string Key => UniqueID;

        [Tooltip("Name used in the database for that Item, used by save system so no [spaces]")]
        public string UniqueID = "DefaultID";

        [Tooltip("Display name that will be visible in-game, allowed to have [spaces] in the name")]        
        public string DisplayName;
        public ItemType Type;
        public Sprite ItemSprite;
        public int MaxStackSize = 32;
        public bool Consumable = true;

        // NOTE: This is just string for a tooltip. You must place Tooltip_Trigger on
        //       whatever you want to have a tooltip, and grab the item's Tooltip strings
        //       and set that Tooltip_Trigger's strings to these string.
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


        // CanUse needs to see if it can be used on this tile, Use is the abstract logic of using that item
        public abstract bool CanUse(Vector3Int target);
        public abstract bool Use(Vector3Int target, GameObject gameObject);

        // Override this for item that needs a specific target (Like placing stationary box on ground tiles)
        public virtual bool NeedTarget()
        {   return false;   }
                
        public bool IsStackable()
        {   return MaxStackSize > 1;    }

        // This is for scoring food sources for creatures. Inedibles will return -1.
        public virtual float hasFoodValue()
        {   return -1; }
        
        // Just a method to play audioclips when you Use an item
        public void PlaySound()
        {
            if(UseSound.Length == 0)
            {
                Debug.LogWarning($"Missing sound clips for item {DisplayName}");
                return;
            }

            // Sends all audioclips to master audio
            foreach(AudioClip clip in UseSound)
            {   AudioManager.Instance.PlaySoundFX(clip);   }
        }

            // Just a method to play audioclips when you Use an item
        public void PlaySound(float volume)
        {
            if(UseSound.Length == 0)
            {
                Debug.LogWarning($"Missing sound clips for item {DisplayName}");
                return;
            }

            // Sends all audioclips to master audio
            foreach(AudioClip clip in UseSound)
            {   AudioManager.Instance.PlaySoundFX(clip, volume);   }
        }
    }

}