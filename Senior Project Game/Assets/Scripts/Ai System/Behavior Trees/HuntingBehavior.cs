
using System.Linq;
using Cyrcadian.BehaviorTrees;
using UnityEditor;
using UnityEngine;

namespace Cyrcadian.UtilityAI
{
    // SAVED AS SCRIPTABLE OBJECT SO WE CAN REFERENCE IN INSPECTOR WHAT TYPES THERE ARE. YOU MUST CREATE AN ASSETMENU FOR EACH BEHAVIOR.
    public abstract class HuntingBehavior : ScriptableObject
    {   
        // When attached in FindFood, parent is up to 4 + 1 for the newly instanced Selector node from Execute. 
        // Accessing the dataContext requires --------------> parent.parent.parent.parent.parent.SetData()
        // GetData() recursively goes up the Node tree so no need to label parent, usually.

        // Double / Triple check that FindFood Action SetData correctly to the right parent if there are any issues.


        // Format: Execute creates a new Selector node, which gets returned at the end.
        public abstract Node Execute(CreatureController myCreature);
    }

    public class CheckAttackRange : Node
    {

    }














[CustomPropertyDrawer(typeof(HuntingBehavior), true)]
public class HuntingBehaviorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get all ScriptableObject assets of the specified type
        HuntingBehavior[] huntingBehaviors = Resources.FindObjectsOfTypeAll<HuntingBehavior>();

        // Filter ScriptableObjects by specific types derived from HuntingBehavior
        HuntingBehavior[] filteredHuntingBehaviors = huntingBehaviors.Where(obj => obj.GetType().IsSubclassOf(typeof(HuntingBehavior))).ToArray();

        // Display a dropdown list for selecting the filtered ScriptableObject
        int selectedIndex = -1;
        string[] options = new string[filteredHuntingBehaviors.Length + 1];
        options[0] = "None";
        for (int i = 0; i < filteredHuntingBehaviors.Length; i++)
        {
            options[i + 1] = filteredHuntingBehaviors[i].name;
            if (property.objectReferenceValue == filteredHuntingBehaviors[i])
            {
                selectedIndex = i + 1;
            }
        }

        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, options);

        if (selectedIndex == 0)
        {
            property.objectReferenceValue = null;
        }
        else if (selectedIndex > 0)
        {
            property.objectReferenceValue = filteredHuntingBehaviors[selectedIndex - 1];
        }

        EditorGUI.EndProperty();
    }
}
}
