using UnityEngine;

namespace Cyrcadian.temp
{
    public abstract class Item : ScriptableObject, IDatabaseEntry
    {
        public string Key => UniqueID;

        [Tooltip("Name used in the database for that Item, used by save system")]
        public string UniqueID = "DefaultID";
        
        public string DisplayName;
        public Sprite ItemSprite;
        public int MaxStackSize = 10;
        public bool Consumable = true;
        public int BuyPrice = -1;

        [Tooltip("Prefab that will be instantiated in the player 'hand' when this is equipped")]
        // 'Hand' in this context means it is what will be used when you left click
        // Typically this prefab will just be a transform and a spriterenderer, however we can get creative
        public GameObject VisualPrefab;

        [Tooltip("This animation played by the player's animator when 'Using' this item")]
        public string PlayerAnimatorTriggerUse = "GenericToolSwing";
        
        [Tooltip("Sounds triggered when using the item")]
        public AudioClip[] UseSound;

        public abstract bool CanUse(Vector3Int target);
        public abstract bool Use(Vector3Int target);

        //override this for item that does not need a target (like Product, they can be eaten anytime)
        public virtual bool NeedTarget()
        {
            return true;
        }
    }
}