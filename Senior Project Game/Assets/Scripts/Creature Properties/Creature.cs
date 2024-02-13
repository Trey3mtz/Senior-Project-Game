using System.Collections.Generic;
using Cyrcadian.Items;
using Cyrcadian.UtilityAI;
using UnityEditor.Animations;
using UnityEngine;

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


        [Header("Name, Behavior, and the Cyrcadian Rythm of a creature")]
        [Tooltip("Name for this creature")]        
        public string CreatureName;
        [Tooltip("Skittish is easily scared, Passive will loaf around, and Aggressive will look for fights")]
        public BehaviorType Behavior;
        [Tooltip("Active during Night, Day, Twilight, or All Times")]
        public CyrcadianRhythm CircadianRhythm;
        [Tooltip("Diet consists of Plants, Meat, and Opporunistic meals")]
        public DietarySystem Diet;

        [Tooltip("Stats will influence decision making")]
        public Creature_Stats Stats;
        
        [Space]

        [Header("Visual Components that a creature should have")]  
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

        [Header("The AI components of a creature")]
        public CreatureController CreatureController;
        public Action[] ListOfPossibleActions;

        // Use the Key
        [Header("List of possible attacks")]
        [Tooltip("List of possible attacks they can do")]
        public List<Attack> PossibleAttacks;
        

        [Tooltip("This is a generic gameobject prefab, which will hold all of a Creature's components")]
        public GameObject CreaturePrefab;


        [Header("Creature Sounds")]
        [Tooltip("In Order it is: Moving, Hurt, Death")]
        public AudioClip[] CreatureSounds;

        [Header("Spawnable Loot Table")]
        public SpawnableLoot[] LootTable;


        // This is handled by the maximum and minimum amount of item Meat dropped from their Loot Table.
        // Therefore, it is logic that must be executed later.
        // The more likely meat will fall out, and the more high quality that meat, the better the ProteinScore.
        // It is used for Consideration in Carnivore's and Omnivores target in hunting.

        public float GetProteinScore()
        {
            float ProteinScore = 0f;
            // If this creature drops food, increase ProteinScore (for predators to look at). 
            // Weighted so HighestAmount holds more influence than the LowestAmount, unless drop rate is 100%. 
            for(int i = 0; i < LootTable.Length; i++)
            {
                if( LootTable[i].item.Type == Item.ItemType.Food)
                {
                    float chance = Mathf.Clamp01(LootTable[i].chance); 
                    ProteinScore += (LootTable[i].highestDropAmount + (LootTable[i].lowestDropAmount  * chance)) * chance;
                }
            }

            return ProteinScore;
        }
    }
}
