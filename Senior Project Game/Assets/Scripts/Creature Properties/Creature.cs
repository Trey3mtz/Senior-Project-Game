using System.Collections.Generic;
using Cyrcadian.Items;
using Cyrcadian.UtilityAI;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEditor.Search;
using System;
using System.Linq;



namespace Cyrcadian.Creatures
{
    [CreateAssetMenu(fileName = "Creature", menuName = "2D Survival/Creature")]
    public class Creature : ScriptableObject
    {
        // Here are some behavior types to give to a variety of creatures
        // Even those creatures of the same species can have a different behavior type
        // Skittish = Easily scared (influence the Flee action?)
        // Passive = will likely not others (influence the Idle action?)
        // Aggressive = will probably attack others ( No action yet for attacking, maybe this will help? )
        public enum BehaviorType{
            Skittish,
            Passive,
            Aggressive,
            RandomizeAtBirth
        }

        // Here are Cyrcadian Rythm types to know what hours the creature will be active
        //                              Nocturnal:      Active during Night Time
        //                                Diurnal:      Active during Day Time
        //                            Crepuscular:      Active during Twilight
        //                             Cathemeral:      Active at All Times
        public enum CyrcadianRhythm{
            Nocturnal,
            Diurnal,
            Crepuscular, 
            Cathemeral 
        }

        public enum DietarySystem{
            Herbivore,
            Carnivore,
            Omnivore
        }
        
    void Awake()
    {
        // Make sure our list of attacks are always sorted by their range
        if(ListOfPossibleAttacks.Count != 0)
            ListOfPossibleAttacks.Sort(0, ListOfPossibleAttacks.Capacity, Comparer<Attack>.Create((a,b) => CompareAttackRange(a,b)));
    }
    public int CompareAttackRange(Attack x, Attack y)
    {
        if (x == null || y == null)
        {
            return 0;
        }
        return x.Range.CompareTo(y.Range);
    }
  
        [Tooltip("Name for this creature")]        
        public string CreatureName;
        [Tooltip("Skittish is easily scared, Passive will loaf around, and Aggressive will look for fights")]
        public BehaviorType Behavior;
        [Tooltip("Active during Night, Day, Twilight, or All Times")]
        public CyrcadianRhythm CircadianRhythm;

        [Space]
  
        [Tooltip("Am I a plant eater or a meat eater?")]
        public DietarySystem Diet;
   
        [Tooltip("What types of food items should I be able to eat?")]
        public Food.FoodType[] myDietSelection;

        [Tooltip("What GameObject Tags should I keep an eye for food?")]
        [TagSelector] public string[] PossibleFoodSources;


        public HuntingBehavior Hunting_Behavior;

        [Space]

        [Tooltip("Stats will influence decision making")]
        public Creature_Stats Stats;
        
        [Space]

 
        public Sprite Sprite;
        public AnimatorController AnimatorController;
        [Tooltip("This will override a sprites material if filled")]
        public Material MatOverride;

        [Tooltip("This value will be added to the Shadows local Y position")]
        public float ShadowHeightAdjust;
        [Tooltip("This value will be added by the Shadows local X scale")]
        public float ShadowLengthAdjust;

        [Tooltip("The physics of a creature")]
        public Rigidbody2D rb;
        public Collider2D collider2D;

      
        public CreatureController CreatureController;
        public UtilityAI.Action[] ListOfPossibleActions;

        // Use the Key
     
        [Tooltip("List of possible attacks they can do")]
        public List<Attack> ListOfPossibleAttacks;
        

        [Tooltip("This is a generic gameobject prefab, which will hold all of a Creature's components")]
        public GameObject CreaturePrefab;



        [Tooltip("In Order it is: Moving, Hurt, Death")]
        public AudioClip[] CreatureSounds;

        
        public SpawnableLoot[] LootTable;


        // This is handled by the maximum and minimum amount of item Meat dropped from their Loot Table.
        // Therefore, it is logic that must be executed later.
        // The more likely meat will fall out, and the more high quality that meat, the better the FoodScore.
        // It is used for Consideration in Carnivore's and Omnivores target in hunting.
        // Might make a GetProteinScore specifically for Carnivores and just ignore non-animal based food.
        public float GetFoodScore()
        {
            float FoodScore = 0f;
            // If this creature drops food, increase FoodScore (for predators to look at). 
            // Weighted so HighestAmount holds more influence than the LowestAmount, unless drop rate is 100%. 
            for(int i = 0; i < LootTable.Length; i++)
            {  
                if( LootTable[i].item.Type == Item.ItemType.Food)
                {
                    Food thisItem = LootTable[i].item as Food;
                    float chance = Mathf.Clamp01(LootTable[i].chance); 
                    FoodScore += thisItem.GetFoodValue() * (LootTable[i].highestDropAmount + (LootTable[i].lowestDropAmount  * chance)) * chance;
                }
            }
            
            return FoodScore;
        }

        // Returns my array of FoodSelections
        public Food.FoodType[] GetFoodSelection()
        {     
            return myDietSelection;
        }
    }


    [CustomEditor(typeof(Creature))]
    public class CreatureEditor : Editor
    {

            private GUIStyle darkBoxStyle;
            private GUIStyle centeredGreenLabel;
            private GUIStyle centeredLightLabel; 


            private void InitializeStyles()
            {
                darkBoxStyle = new GUIStyle(GUI.skin.box);
                darkBoxStyle.normal.background = MakeTex(3, 2, new Color(0.15f, 0.15f, 0.15f, 1f)); // Adjust the color values to make it darker

                centeredGreenLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                centeredLightLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                //centeredLightLabel.normal.background = MakeTex(3, 2, new Color(0.18f, 0.18f, 0.18f, .5f));

                centeredGreenLabel.normal.textColor = new Color(0.5f,.85f,0.5f);
                centeredGreenLabel.fontSize += 4;

                centeredLightLabel.fontStyle = FontStyle.Bold;
                centeredLightLabel.fontSize += 5;
                centeredLightLabel.normal.textColor = new Color(1f,1f,1f, .9f);

            }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            if (darkBoxStyle == null  || centeredGreenLabel == null || centeredLightLabel == null)
            {
                InitializeStyles();
            }
            Creature thisCreature = (Creature)target;

            serializedObject.Update();

            EditorGUILayout.Space();     
            EditorGUILayout.LabelField("Creature Asset:   " + thisCreature.CreatureName , centeredLightLabel);
            EditorGUILayout.Space();    EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CreatureName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Behavior"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CircadianRhythm"));

            EditorGUILayout.Space();
            EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(darkBoxStyle);
            EditorGUILayout.LabelField("Dietary Settings", centeredGreenLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Diet"));

        
                if (thisCreature.Diet == Creature.DietarySystem.Herbivore)
                {

                }
                else if (thisCreature.Diet == Creature.DietarySystem.Carnivore)
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;

                    SerializedProperty huntingBehaviorProperty = serializedObject.FindProperty("Hunting_Behavior");
                        if (huntingBehaviorProperty != null)
                        {
                            EditorGUILayout.PropertyField(huntingBehaviorProperty);
                        }
                        else
                        {
                            Debug.LogWarning("Hunting_Behavior property not found in serializedObject.");
                        }
                   
                    EditorGUI.indentLevel--;
                }
                else if (thisCreature.Diet == Creature.DietarySystem.Omnivore)
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;

      
                    SerializedProperty huntingBehaviorProperty = serializedObject.FindProperty("Hunting_Behavior");
                        if (huntingBehaviorProperty != null)
                        {
                            EditorGUILayout.PropertyField(huntingBehaviorProperty);
                        }
                        else
                        {
                            Debug.LogWarning("Hunting_Behavior property not found in serializedObject.");
                        }
                    EditorGUI.indentLevel--;
                }
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("myDietSelection"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("PossibleFoodSources"), true);



            EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(darkBoxStyle);     
            EditorGUILayout.LabelField("My Stats", centeredGreenLabel);
   
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Stats"));

            EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("Visual Components that a creature should have", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Sprite"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimatorController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MatOverride"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShadowHeightAdjust"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShadowLengthAdjust"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rb"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("collider2D"));

            EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(darkBoxStyle);   
            EditorGUILayout.LabelField("The AI components of a creature", centeredGreenLabel);
            EditorGUILayout.Space();
           // EditorGUILayout.PropertyField(serializedObject.FindProperty("CreatureController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ListOfPossibleActions"));

            EditorGUILayout.Space();

            //EditorGUILayout.LabelField("List of possible attacks", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ListOfPossibleAttacks"));
        

            EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Creature Sounds", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CreatureSounds"), true);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Spawnable Loot Table", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LootTable"), true);

            serializedObject.ApplyModifiedProperties();
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

namespace Cyrcadian
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    
#endif


    public class TagSelectorAttribute : PropertyAttribute
    {
        public bool UseDefaultTagFieldDrawer = false;
    }
    
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);
    
                var attrib = this.attribute as TagSelectorAttribute;
    
                if (attrib.UseDefaultTagFieldDrawer)
                {
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                }
                else
                {
                    //generate the taglist + custom tags
                    List<string> tagList = new List<string>();
                    tagList.Add("<NoTag>");
                    tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
                    string propertyString = property.stringValue;
                    int index = -1;
                    if(propertyString =="")
                    {
                        //The tag is empty
                        index = 0; //first index is the special <notag> entry
                    }
                    else
                    {
                        //check if there is an entry that matches the entry and get the index
                        //we skip index 0 as that is a special custom case
                        for (int i = 1; i < tagList.Count; i++)
                        {
                            if (tagList[i] == propertyString)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                    
                    //Draw the popup box with the current selected index
                    index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());
    
                    //Adjust the actual string value of the property based on the selection
                    if(index==0)
                    {
                        property.stringValue = "";
                    }
                    else if (index >= 1)
                    {
                        property.stringValue = tagList[index];
                    }
                    else
                    {
                        property.stringValue = "";
                    }
                }
    
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }

}
