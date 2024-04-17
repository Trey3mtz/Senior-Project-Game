using System.Linq;
using Unity.Entities;
using UnityEditor;
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
        public virtual int GetFoodValue()
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
[CustomPropertyDrawer(typeof(Item), true)]
public class ScriptableObjectDrawer : PropertyDrawer
{private GUIStyle dropdownStyle;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (dropdownStyle == null)
        {
            dropdownStyle = new GUIStyle(EditorStyles.popup);
            dropdownStyle.normal.background = MakeTex(1, 1, new Color(0.3f, 0.3f, 0.3f, 1f)); // Set the box color
        }
        EditorGUI.BeginProperty(position, label, property);

        // Get all ScriptableObject assets of the specified type
        ScriptableObject[] scriptableObjects = Resources.FindObjectsOfTypeAll<ScriptableObject>();

        // Filter ScriptableObjects by specific types derived from ScriptableObject
        ScriptableObject[] filteredScriptableObjects = scriptableObjects.Where(obj => obj.GetType().IsSubclassOf(typeof(Item))).ToArray();

        // Display a dropdown list for selecting the filtered ScriptableObject
        int selectedIndex = -1;
        string[] options = new string[filteredScriptableObjects.Length + 1];
        options[0] = "None";
        for (int i = 0; i < filteredScriptableObjects.Length; i++)
        {
            options[i + 1] = filteredScriptableObjects[i].name;
            if (property.objectReferenceValue == filteredScriptableObjects[i])
            {
                selectedIndex = i + 1;
            }
        }

        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, options );

        if (selectedIndex == 0)
        {
            property.objectReferenceValue = null;
        }
        else if (selectedIndex > 0)
        {
            property.objectReferenceValue = filteredScriptableObjects[selectedIndex - 1];
        }

        EditorGUI.EndProperty();
    }

        private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = color;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
}