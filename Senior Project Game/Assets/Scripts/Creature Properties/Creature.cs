using Cyrcadian.Items;
using Cyrcadian.UtilityAI;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

namespace Cyrcadian.Creatures
{
    [CreateAssetMenu(fileName = "Creature", menuName = "2D Survival/Creature")]
    public class Creature : ScriptableObject
    {
        // Here are some behavior types to give to a variety of creatures
        // Even those creatures of the same species can have a different behavior type
        // Maybe a predator will sometimes be skittish
        public enum BehaviorType{
            Skittish,
            Passive,
            Aggressive,
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


        [Header("Name, Behavior, and the Cyrcadian Rythm of a creature")]
        [Tooltip("Name for this creature")]        
        public string CreatureName;
        [Tooltip("Skittish is easily scared, Passive will loaf around, and Aggressive will look for fights")]
        public BehaviorType Behavior;
        [Tooltip("Active during Night, Day, Twilight, or All Times")]
        public CyrcadianRhythm CircadianRhythm;

        [Tooltip("Stats will influence decision making")]
        public Creature_Stats Stats;
        
        [Space]

        [Header("Visual Components that a creature should have")]  
        public Sprite Sprite;
        public AnimatorController AnimatorController;

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
        

        [Tooltip("This is a generic gameobject prefab, which will hold all of a Creature's components")]
        public GameObject CreaturePrefab;


        [Header("Creature Sounds")]
        [Tooltip("In Order it is: Moving, Hurt, Death")]
        public AudioClip[] CreatureSounds;

        [Header("Spawnable Loot Table")]
        public SpawnableLoot[] LootTable;

    }
}
